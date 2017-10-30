using System.Web.Mvc;

namespace MVC.Areas.FileProcessing
{
    public class FileProcessingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FileProcessing";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FileProcessing_default",
                "FileProcessing/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}