using System.Linq;
using System.Web.Mvc;
using _25DH110740_MyStore.Models;
using PagedList;

namespace _25DH110740_MyStore.Areas.Admin.Controllers
{
    public class OrderController : AdminBaseController
    {
        public ActionResult Index(string status, string searchTerm, int? customerId, int? page)
        {
            var q = db.Orders.Include("Customer").AsQueryable();
            if (!string.IsNullOrEmpty(status)) q = q.Where(o => o.PaymentStatus == status);
            if (customerId.HasValue) { q = q.Where(o => o.CustomerId == customerId.Value); ViewBag.Customer = db.Customers.Find(customerId.Value); }
            if (!string.IsNullOrWhiteSpace(searchTerm)) q = q.Where(o => o.Customer.CustomerName.Contains(searchTerm));
            ViewBag.Status = status; ViewBag.SearchTerm = searchTerm; ViewBag.CustomerId = customerId;
            return View(q.OrderByDescending(o => o.OrderDate).ToPagedList(page ?? 1, 10));
        }

        public ActionResult Details(int id)
        {
            var o = db.Orders.Include("Customer").Include("OrderDetails.Product").FirstOrDefault(x => x.OrderId == id);
            return o == null ? (ActionResult)HttpNotFound() : View(o);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int id, string status)
        {
            var o = db.Orders.Include("OrderDetails").FirstOrDefault(x => x.OrderId == id);
            if (o == null || !OrderStatus.All.Contains(status)) return HttpNotFound();
            if (status == OrderStatus.Cancelled && o.PaymentStatus != OrderStatus.Cancelled)
                foreach (var d in o.OrderDetails) { var p = db.Products.Find(d.ProductId); if (p != null) { p.Stock += d.Quantity; p.Sold = System.Math.Max(0, p.Sold - d.Quantity); } }
            o.PaymentStatus = status; db.SaveChanges();
            return RedirectToAction("Details", new { id });
        }
    }
}
