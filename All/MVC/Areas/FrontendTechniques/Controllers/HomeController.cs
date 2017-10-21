using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC.Areas.FrontendTechniques.Models;

namespace MVC.Areas.FrontendTechniques.Controllers
{
    public class HomeController : Controller
    {
        // GET: FrontendTechniques/Home
        public ActionResult Index()
        {
            var list = new List<CheckModel>
            {
                new CheckModel{Id = 1, Name = "Aquafina", Checked = false},
                new CheckModel{Id = 2, Name = "Mulshi Springs", Checked = false},
                new CheckModel{Id = 3, Name = "Alfa Blue", Checked = false},
                new CheckModel{Id = 4, Name = "Atlas Premium", Checked = false},
                new CheckModel{Id = 5, Name = "Bailley", Checked = false},
                new CheckModel{Id = 6, Name = "Bisleri", Checked = false},
                new CheckModel{Id = 7, Name = "Himalayan", Checked = false},
                new CheckModel{Id = 8, Name = "Cool Valley", Checked = true},
                new CheckModel{Id = 9, Name = "Dew Drops", Checked = false},
                new CheckModel{Id = 10, Name = "Dislaren", Checked = false},

            };
            return View(list);
        }
        [HttpPost]
        public ActionResult Index(List<CheckModel> list)
        {
            var data = list.FindAll(x => x.Checked);

            return View();
        }

        // Microsoft.jQuery.Unobtrusive.Ajax
    }
}