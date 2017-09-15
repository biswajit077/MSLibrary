using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC.Models;

namespace MVC.Controllers
{
    public class DropDownListDemoController : Controller
    {
        #region Approach 1: Populate DropDownList by SelecteListItem using Enum
        public ActionResult Index()
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