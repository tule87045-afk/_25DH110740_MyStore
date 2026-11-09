using System.Linq;
using System.Web.Mvc;
using _25DH110740_MyStore.Models.ViewModel;
using PagedList;

namespace _25DH110740_MyStore.Controllers
{
    public class CartController : BaseController
    {
       
        public ActionResult Index(int? page)
        {
            var cart = GetCartService().GetCart();
            var catIds = cart.Items.Select(i => i.CategoryId).Distinct().ToList();
            var inCart = cart.Items.Select(i => i.ProductId).ToList();
            cart.SimilarProducts = db.Products
                .Where(p => catIds.Contains(p.CategoryId) && !inCart.Contains(p.ProductId))
                .OrderByDescending(p => p.Sold).ToList().ToPagedList(page ?? 1, 4);
            return View(cart);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddItem(int id, int quantity = 1, string returnUrl = null)
        {
            var p = db.Products.Include("Category").FirstOrDefault(x => x.ProductId == id);
            if (p == null) return HttpNotFound();
            GetCartService().Add(p, quantity);
            TempData["Message"] = "Đã thêm \"" + p.ProductName + "\" vào giỏ hàng";
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RemoveItem(int id)
        {
            GetCartService().Remove(id);
            return RedirectToAction("Index");
        }

        public ActionResult ClearCart()
        {
            GetCartService().Clear();
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            GetCartService().UpdateQuantity(id, quantity);
            return RedirectToAction("Index");
        }
    }
}
