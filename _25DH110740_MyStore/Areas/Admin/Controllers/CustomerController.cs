using System.Linq;
using System.Web.Mvc;
using PagedList;

namespace _25DH110740_MyStore.Areas.Admin.Controllers
{
    public class CustomerController : AdminBaseController
    {
        public ActionResult Index(string searchTerm, string sortOrder, int? page)
        {
            var q = db.Customers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
                q = q.Where(c => c.CustomerName.Contains(searchTerm) || c.Username.Contains(searchTerm) || c.CustomerEmail.Contains(searchTerm) || c.CustomerPhone.Contains(searchTerm));
            switch (sortOrder)
            {
                case "name_desc": q = q.OrderByDescending(c => c.CustomerName); break;
                case "username": q = q.OrderBy(c => c.Username); break;
                case "orders_desc": q = q.OrderByDescending(c => c.Orders.Count); break;
                default: q = q.OrderBy(c => c.CustomerName); break;
            }
            ViewBag.SearchTerm = searchTerm; ViewBag.SortOrder = sortOrder;
            return View(q.ToPagedList(page ?? 1, 10));
        }
    }
}
