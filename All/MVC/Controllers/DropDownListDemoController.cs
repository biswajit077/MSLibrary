using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC.Models;
using MVC.Utilities;
using PagedList;

namespace MVC.Controllers
{
    public class DropDownListDemoController : Controller
    {
        private readonly DbContexClass _db;
        public DropDownListDemoController()
        {
            this._db = new DbContexClass();
        }
        #region Approach 1: Populate DropDownList by SelecteListItem using Enum
        public ActionResult PersonListIndex()
        {
            ActionModel model = new ActionModel();
            IEnumerable<ActionType> actionTypes = Enum.GetValues(typeof(ActionType)).Cast<ActionType>();
            model.ActionsList = from action in actionTypes
                select new SelectListItem
                {
                    Text = action.ToString(),
                    Value = ((int)action).ToString()
                };

            return View(model);
        }
        #endregion

        #region Approach 2: Populate the DropDownList by HTML Helper method for the Enum
        public ActionResult ActionTypes()
        {
            ActionTypeModel model = new ActionTypeModel();
            return View(model);
        }


        #endregion

        #region Pagination
        [HttpGet]
        public ActionResult PersonList(string sortOrder, string CurrentSort, int? page)
        {
            int pageSize = 5;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            ViewBag.CurrentSort = sortOrder;

            sortOrder = String.IsNullOrEmpty(sortOrder) ? "ID" : sortOrder;

            IPagedList<Person> persons = null;

            switch (sortOrder)
            {
                case "Name":
                    if (sortOrder.Equals(CurrentSort))
                        persons = _db.Persons.OrderByDescending
                            (m => m.Name).ToPagedList(pageIndex, pageSize);
                    else
                        persons = _db.Persons.OrderBy
                            (m => m.Name).ToPagedList(pageIndex, pageSize);
                    break;
                case "Address":
                    if (sortOrder.Equals(CurrentSort))
                        persons = _db.Persons.OrderByDescending
                            (m => m.Address).ToPagedList(pageIndex, pageSize);
                    else
                        persons = _db.Persons.OrderBy
                            (m => m.Name).ToPagedList(pageIndex, pageSize);
                    break;

                case "ContactNumber":
                    if (sortOrder.Equals(CurrentSort))
                        persons = _db.Persons.OrderByDescending
                            (m => m.ContactNumber).ToPagedList(pageIndex, pageSize);
                    else
                        persons = _db.Persons.OrderBy
                            (m => m.ContactNumber).ToPagedList(pageIndex, pageSize);
                    break;

                case "ID":
                    if (sortOrder.Equals(CurrentSort))
                        persons = _db.Persons.OrderByDescending
                            (m => m.ID).ToPagedList(pageIndex, pageSize);
                    else
                        persons = _db.Persons.OrderBy
                            (m => m.ID).ToPagedList(pageIndex, pageSize);
                    break;
                case "Default":
                    persons = _db.Persons.OrderBy
                        (m => m.ID).ToPagedList(pageIndex, pageSize);
                    break;
            }
            return View(persons);
            //IPagedList<Person> p = dataList.ToList().ToPagedList(1,3);
            //or
            //IPagedList<Person> p = dataList.AsEnumerable().ToPagedList(1,3);
            //return View();
        }
        #endregion
        #region Customize 
        public enum ActionType
        {
            
            Create = 1, [EnumDisplayName(DisplayName = "Super Admin")]
            Read = 2,
            Update = 3,
            Delete = 4
        }
        public class EnumDisplayNameAttribute : Attribute
        {
            private string _displayName;
            public string DisplayName
            {
                get
                {
                    return _displayName;
                }
                set
                {
                    _displayName = value;
                }
            }
        }
        #endregion
    }
}