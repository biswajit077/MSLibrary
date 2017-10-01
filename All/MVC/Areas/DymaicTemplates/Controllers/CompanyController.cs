using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelClass.Model;

namespace MVC.Areas.DymaicTemplates.Controllers
{
    public class CompanyController : Controller
    {
        // GET: DymaicTemplates/Company
        public ActionResult Index()
        {
            var newCompany = new Company();
            return View(newCompany);
        }
        public ActionResult AddNewEmployee()
        {
            var employee = new Employee();
            return PartialView("_Employee", employee);
        }
    }
}