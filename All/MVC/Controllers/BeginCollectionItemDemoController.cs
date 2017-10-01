using System.Web.Mvc;
using ModelClass.ViewModel;

namespace MVC.Controllers
{
    public class BeginCollectionItemDemoController : Controller
    {
        // GET: BeginCollectionItemDemo
        public ActionResult Index()
        {
            var homeViewModel = new HomeViewModel();
            homeViewModel.PopulateClassicBooks();

            return View(homeViewModel);
        }

        [HttpPost]
        public ActionResult Index(HomeViewModel homeViewModel)
        {
            return View(homeViewModel);
        }

        public ActionResult CreateNewBook()
        {
            var bookViewModel = new BookViewModel();

            return PartialView("~/Views/Shared/EditorTemplates/BookViewModel.cshtml", bookViewModel);
        }
    }
}