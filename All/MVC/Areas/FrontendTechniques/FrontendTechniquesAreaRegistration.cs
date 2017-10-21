using System.Web.Mvc;

namespace MVC.Areas.FrontendTechniques
{
    public class FrontendTechniquesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FrontendTechniques";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FrontendTechniques_default",
                "FrontendTechniques/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}