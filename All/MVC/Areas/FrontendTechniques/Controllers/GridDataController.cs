using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC.Utilities;


namespace MVC.Areas.FrontendTechniques.Controllers
{
    public class GridDataController : Controller
    {
        private readonly DbContext _db;

        public GridDataController()
        {
            _db = new DbContexClass();
        }
        // GET: FrontendTechniques/GridData
        public ActionResult Index()
        {
            
            return View();
        }
    }
}