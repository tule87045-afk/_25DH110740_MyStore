using System.Linq;
using System.Web.Mvc;
using _25DH110740_MyStore.Models.ViewModel;

namespace _25DH110740_MyStore.Controllers
{
    
    public class SharedController : BaseController
    {
        [ChildActionOnly]
        public ActionResult CategoryMenu()
        {
            return PartialView("_CategoriesPV", db.Categories.OrderBy(c => c.CategoryName).ToList());
        }

        [ChildActionOnly]
        public ActionResult PersonalMenu()
        {
            var vm = new PersonalMenuVM
            {
                IsLoggedIn = User.Identity.IsAuthenticated,
                Username = User.Identity.Name,
                CartCount = GetCartService().GetCart().Count
            };
            return PartialView("_PersonalMenuPV", vm);
        }
    }
}
