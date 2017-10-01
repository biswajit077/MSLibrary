using System.Web.Mvc;

namespace MVC.Areas.DymaicTemplates
{
    public class DymaicTemplatesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DymaicTemplates";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DymaicTemplates_default",
                "DymaicTemplates/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}