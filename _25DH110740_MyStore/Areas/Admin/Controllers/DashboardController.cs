using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using _25DH110740_MyStore.Models;
using _25DH110740_MyStore.Models.ViewModel;

namespace _25DH110740_MyStore.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        public ActionResult Index()
        {
            var valid = db.Orders.Where(o => o.PaymentStatus != OrderStatus.Cancelled);
            var byMonth = valid.ToList().GroupBy(o => o.OrderDate.ToString("MM/yyyy"))
                .Select(g => new KeyValuePair<string, decimal>(g.Key, g.Sum(o => o.TotalAmount))).ToList();
            var vm = new DashboardVM
            {
                ProductCount = db.Products.Count(),
                CustomerCount = db.Customers.Count(),
                OrderCount = db.Orders.Count(),
                Revenue = valid.Any() ? valid.Sum(o => o.TotalAmount) : 0,
                OrdersByStatus = OrderStatus.All.Select(s => new KeyValuePair<string, int>(s, db.Orders.Count(o => o.PaymentStatus == s))).ToList(),
                RevenueByMonth = byMonth,
                TopProducts = db.Products.Include("Category").OrderByDescending(p => p.Sold).Take(5).ToList()
            };
            return View(vm);
        }
    }
}
