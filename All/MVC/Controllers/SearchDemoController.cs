using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC.Models;
using MVC.Utilities;
using PagedList;

namespace MVC.Controllers
{
    public class SearchDemoController : Controller
    {
        private readonly DbContexClass _db;
        public SearchDemoController()
        {
            _db = new DbContexClass();
        }
        // GET: SearchDemo
        public ActionResult Index(int? itemsPerPage, int? page)
        {
            ViewBag.CurrentItemsPerPage = itemsPerPage;
            var data = _db.Persons.ToList();
            //Search a list of objects for any and all matches
            var usersList = new List<Person>();
            var filteredUsersList = data.WhereAtLeastOneProperty((string s) => s.Contains("Sonya ")).ToList();
            //or
            var filteredUsersList1 = usersList.WhereAtLeastOneProperty((string s) => s == "Nicole Franco");


            return View(data.ToPagedList(page ?? 1, itemsPerPage ?? 30));
        }

        public ActionResult UpdateMultipleRows()
        {
            //var  data = _db.Persons.ToList();
            //data.ForEach(a => a.Name = "00"+a.Name);
            //_db.SaveChanges();

            //or
            List<Person> ls = new List<Person>()
            {
                new Person()
                {
                    ID = 1,
                    Name = "Nicole Franco",
                    Address = "78 Fabien Freway",
                    ContactNumber = "16780"
                }
            };
                _db.Persons
                    .Where(x =>ls.Contains(x))
                    .ToList()
                    .ForEach(a => a.Name = "00"+a.Name);
                    //.ForEach(a => a.Name = "00"+a.Name);
                    
                _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}