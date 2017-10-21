using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC.Utilities;
using PagedList;

namespace MVC.Controllers
{
    public class PageListDemoController : Controller
    {
        private readonly DbContexClass _db;
        public PageListDemoController()
        {
            _db = new DbContexClass();
        }
        // GET: PageListDemo
        public ActionResult Index(int? itemsPerPage, int? page)
        {
            ViewBag.CurrentItemsPerPage = itemsPerPage;
            var data = _db.Persons.ToList();

            return View(data.ToPagedList(page??1, itemsPerPage??30));
        }
    }
}