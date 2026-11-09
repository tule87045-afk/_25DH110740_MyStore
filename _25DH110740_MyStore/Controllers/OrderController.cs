using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using _25DH110740_MyStore.Models;
using _25DH110740_MyStore.Models.ViewModel;

namespace _25DH110740_MyStore.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        private decimal ShippingFee(string method, decimal subtotal)
        {
            decimal free = decimal.Parse(ConfigurationManager.AppSettings["FreeShipThreshold"]);
            if (method == "Express") return decimal.Parse(ConfigurationManager.AppSettings["ExpressShippingFee"]);
            return subtotal >= free ? 0 : decimal.Parse(ConfigurationManager.AppSettings["StandardShippingFee"]);
        }

        private List<CartItem> SelectedItems(string ids)
        {
            var cart = GetCartService().GetCart();
            if (string.IsNullOrWhiteSpace(ids)) return cart.Items.ToList();
            var set = ids.Split(',').Select(s => { int n; return int.TryParse(s, out n) ? n : -1; }).ToList();
            return cart.Items.Where(i => set.Contains(i.ProductId)).ToList();
        }

        
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult StartCheckout(int[] selectedIds)
        {
            if (selectedIds == null || selectedIds.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một sản phẩm để mua";
                return RedirectToAction("Index", "Cart");
            }
            return RedirectToAction("Checkout", new { ids = string.Join(",", selectedIds) });
        }

        [HttpGet]
        public ActionResult Checkout(string ids)
        {
            var items = SelectedItems(ids);
            if (items.Count == 0) return RedirectToAction("Index", "Home");
            var c = db.Customers.SingleOrDefault(x => x.Username == User.Identity.Name);
            if (c == null) return RedirectToAction("Login", "Account");
            var vm = new CheckoutVM
            {
                Items = items, Subtotal = items.Sum(i => i.TotalPrice), SelectedIds = ids,
                CustomerName = c.CustomerName, CustomerPhone = c.CustomerPhone, ShippingAddress = c.CustomerAddress,
                DeliveryMethod = "Standard", PaymentMethod = "COD"
            };
            vm.ShippingFee = ShippingFee(vm.DeliveryMethod, vm.Subtotal);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model)
        {
            var items = SelectedItems(model.SelectedIds);
            if (items.Count == 0) return RedirectToAction("Index", "Home");
            var c = db.Customers.SingleOrDefault(x => x.Username == User.Identity.Name);
            if (c == null) return RedirectToAction("Login", "Account");
            if (!string.IsNullOrWhiteSpace(model.NewShippingAddress)) model.ShippingAddress = model.NewShippingAddress.Trim();
            ModelState.Remove("ShippingAddress");
            if (string.IsNullOrWhiteSpace(model.ShippingAddress)) ModelState.AddModelError("ShippingAddress", "Nhập địa chỉ giao hàng");

            model.Items = items; model.Subtotal = items.Sum(i => i.TotalPrice);
            model.ShippingFee = ShippingFee(model.DeliveryMethod, model.Subtotal);
            if (!ModelState.IsValid) return View(model);

            // Kiểm tra tồn kho
            var ids = items.Select(i => i.ProductId).ToList();
            var products = db.Products.Where(p => ids.Contains(p.ProductId)).ToList();
            foreach (var it in items)
            {
                var p = products.FirstOrDefault(x => x.ProductId == it.ProductId);
                if (p == null || p.Stock < it.Quantity)
                {
                    ModelState.AddModelError("", "Sản phẩm \"" + it.ProductName + "\" không đủ hàng trong kho");
                    return View(model);
                }
            }

            var order = new Order
            {
                CustomerId = c.CustomerId, OrderDate = DateTime.Now, TotalAmount = model.Total,
                DeliveryMethod = model.DeliveryMethod == "Express" ? "Giao hàng nhanh" : "Giao hàng tiêu chuẩn",
                PaymentMethod = model.PaymentMethod == "COD" ? "Thanh toán khi nhận hàng"
                              : model.PaymentMethod == "Bank" ? "Chuyển khoản ngân hàng" : "PayPal",
                PaymentStatus = model.PaymentMethod == "COD" ? OrderStatus.Processing : OrderStatus.Pending,
                ShippingAddress = model.ShippingAddress,
                OrderDetails = new List<OrderDetail>()
            };
            foreach (var it in items)
            {
                order.OrderDetails.Add(new OrderDetail { ProductId = it.ProductId, Quantity = it.Quantity, UnitPrice = it.UnitPrice });
                var p = products.First(x => x.ProductId == it.ProductId);
                p.Stock -= it.Quantity; p.Sold += it.Quantity;
            }
            db.Orders.Add(order);
            db.SaveChanges();

            var cs = GetCartService();
            foreach (var it in items) cs.Remove(it.ProductId);
            return RedirectToAction("OrderSuccess", new { id = order.OrderId });
        }

        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders.Include("OrderDetails.Product").Include("Customer")
                          .FirstOrDefault(o => o.OrderId == id && o.Customer.Username == User.Identity.Name);
            if (order == null) return HttpNotFound();
            return View(order);
        }

        
        public ActionResult MyOrder(string status, string searchTerm)
        {
            var q = db.Orders.Include("OrderDetails.Product").Where(o => o.Customer.Username == User.Identity.Name);
            if (!string.IsNullOrWhiteSpace(status)) q = q.Where(o => o.PaymentStatus == status);
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var t = searchTerm.Trim(); int oid;
                if (int.TryParse(t.TrimStart('#'), out oid))
                    q = q.Where(o => o.OrderId == oid || o.OrderDetails.Any(d => d.Product.ProductName.Contains(t)));
                else
                    q = q.Where(o => o.OrderDetails.Any(d => d.Product.ProductName.Contains(t)));
            }
            return View(new MyOrderVM { Status = status, SearchTerm = searchTerm, Orders = q.OrderByDescending(o => o.OrderDate).ToList() });
        }

       
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Reorder(int id)
        {
            var order = db.Orders.Include("OrderDetails.Product.Category")
                          .FirstOrDefault(o => o.OrderId == id && o.Customer.Username == User.Identity.Name);
            if (order == null) return HttpNotFound();
            var cs = GetCartService();
            foreach (var d in order.OrderDetails) cs.Add(d.Product, d.Quantity);
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CancelOrder(int id)
        {
            var order = db.Orders.Include("OrderDetails")
                          .FirstOrDefault(o => o.OrderId == id && o.Customer.Username == User.Identity.Name);
            if (order == null) return HttpNotFound();
            if (order.PaymentStatus == OrderStatus.Pending || order.PaymentStatus == OrderStatus.Processing)
            {
                foreach (var d in order.OrderDetails)
                {
                    var p = db.Products.Find(d.ProductId);
                    if (p != null) { p.Stock += d.Quantity; p.Sold = Math.Max(0, p.Sold - d.Quantity); }
                }
                order.PaymentStatus = OrderStatus.Cancelled;
                db.SaveChanges();
            }
            return RedirectToAction("MyOrder");
        }
    }
}
