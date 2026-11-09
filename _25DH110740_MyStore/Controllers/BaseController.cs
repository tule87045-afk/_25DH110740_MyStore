using System.Web.Mvc;
using _25DH110740_MyStore.Models;
using _25DH110740_MyStore.Models.ViewModel;

namespace _25DH110740_MyStore.Controllers
{
    public abstract class BaseController : Controller
    {
        protected _25DH110740_MyStoreContext db = new _25DH110740_MyStoreContext();

        protected CartService GetCartService() { return new CartService(HttpContext.Session); }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
