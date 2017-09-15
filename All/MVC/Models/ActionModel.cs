using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC.Controllers;

namespace MVC.Models
{
    public class ActionModel
    {
        public ActionModel()
        {
            ActionsList = new List<SelectListItem>();
        }
        [Display(Name = "Actions")]
        public int ActionId { get; set; }
        public IEnumerable<SelectListItem> ActionsList { get; set; }
    }

    public class ActionTypeModel
    {
        [Display(Name = "Actions")]
        public int ActionId { get; set; }
        //public string ActionId { get; set; }
        public DropDownListDemoController.ActionType ActionTypeList { get; set; }
    }
}