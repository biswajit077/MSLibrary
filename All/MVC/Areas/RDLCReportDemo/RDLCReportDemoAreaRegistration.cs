using System.Web.Mvc;

namespace MVC.Areas.RDLCReportDemo
{
    public class RDLCReportDemoAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RDLCReportDemo";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RDLCReportDemo_default",
                "RDLCReportDemo/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}