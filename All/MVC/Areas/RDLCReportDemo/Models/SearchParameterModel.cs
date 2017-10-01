using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.RDLCReportDemo.Models
{
    public class SearchParameterModel
    {
        [Display(Name = "Search By Terrritory")]
        public string Territory
        {
            get;
            set;
        }
        public string Format
        {
            get;
            set;
        }
    }
}