using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Reporting.WinForms;
using MVC.Areas.RDLCReportDemo.Models;

namespace MVC.Areas.RDLCReportDemo.Controllers
{
    public class RdlcReportController : Controller
    {
        // GET: RDLCReportDemo/RdlcReport
        [AllowAnonymous]
        public ActionResult ReportViewer(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ViewResult ReportViewer(SearchParameterModel um)
        {
            return View(um);
        }

        public FileContentResult GenerateAndDisplayReport(string territory, string format)
        {
            LocalReport localReport = new LocalReport();
            LocalReport localReport1 = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Areas/RDLCReportDemo/RDLC/Report1.rdlc");
            localReport1.ReportPath = Server.MapPath("~/Areas/RDLCReportDemo/RDLC/Report2.rdlc");
            IList<WorldModel> customerList = new List<WorldModel>();
            customerList.Add(new WorldModel("Europe", "Sweden", "2001", "1823"));
            customerList.Add(new WorldModel("Europe", "Sweden", "2002", "1234"));
            customerList.Add(new WorldModel("Europe", "Sweden", "2003", "9087"));

            customerList.Add(new WorldModel("Europe", "Denmark", "2001", "6793"));
            customerList.Add(new WorldModel("Europe", "Denmark", "2002", "4563"));
            customerList.Add(new WorldModel("Europe", "Denmark", "2003", "1897"));

            customerList.Add(new WorldModel("Europe", "Norway", "2001", "5632"));
            customerList.Add(new WorldModel("Europe", "Norway", "2002", "9870"));
            customerList.Add(new WorldModel("Europe", "Norway", "2003", "2367"));

            customerList.Add(new WorldModel("Asia", "India", "2001", "1980"));
            customerList.Add(new WorldModel("Asia", "India", "2002", "9765"));
            customerList.Add(new WorldModel("Asia", "India", "2003", "6789"));

            customerList.Add(new WorldModel("Asia", "Japan", "2001", "9871"));
            customerList.Add(new WorldModel("Asia", "Japan", "2002", "2987"));
            customerList.Add(new WorldModel("Asia", "Japan", "2003", "1256"));

            customerList.Add(new WorldModel("North America", "United States", "2001", "9871"));
            customerList.Add(new WorldModel("North America", "United States", "2002", "9871"));
            customerList.Add(new WorldModel("North America", "United States", "2003", "9871"));


            customerList.Add(new WorldModel("North America", "Canada", "2001", "9871"));
            customerList.Add(new WorldModel("North America", "Canada", "2002", "9871"));
            customerList.Add(new WorldModel("North America", "Canada", "2003", "9871"));

            customerList.Add(new WorldModel("North America", "Mexico", "2001", "9871"));
            customerList.Add(new WorldModel("North America", "Mexico", "2002", "9871"));
            customerList.Add(new WorldModel("North America", "Mexico", "2003", "9871"));

            customerList.Add(new WorldModel("South America", "Brazil", "2001", "9871"));
            customerList.Add(new WorldModel("South America", "Brazil", "2002", "9871"));
            customerList.Add(new WorldModel("South America", "Brazil", "2003", "9871"));

            customerList.Add(new WorldModel("South America", "Columbia", "2001", "9871"));
            customerList.Add(new WorldModel("South America", "Columbia", "2002", "9871"));
            customerList.Add(new WorldModel("South America", "Columbia", "2003", "9871"));

            customerList.Add(new WorldModel("South America", "Argentina", "2001", "9871"));
            customerList.Add(new WorldModel("South America", "Argentina", "2002", "9871"));
            customerList.Add(new WorldModel("South America", "Argentina", "2003", "9871"));
            ReportDataSource reportDataSource = new ReportDataSource();
            ReportDataSource reportDataSource1 = new ReportDataSource();
            reportDataSource.Name = "World";
            reportDataSource1.Name = "DataSet1";
            if (territory != null)
            {
                var customerfilterList = from c in customerList
                    where c.Territory == territory
                    select c;


                reportDataSource.Value = customerfilterList;
            }
            else
                reportDataSource.Value = customerList;

            localReport.DataSources.Add(reportDataSource);
            localReport1.DataSources.Add(reportDataSource1);
            string reportType = "Image";
            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType            
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx            
            string deviceInfo = "<DeviceInfo>" +
                                "  <OutputFormat>jpeg</OutputFormat>" +
                                "  <PageWidth>8.5in</PageWidth>" +
                                "  <PageHeight>11in</PageHeight>" +
                                "  <MarginTop>0.5in</MarginTop>" +
                                "  <MarginLeft>1in</MarginLeft>" +
                                "  <MarginRight>1in</MarginRight>" +
                                "  <MarginBottom>0.5in</MarginBottom>" +
                                "</DeviceInfo>";
            Microsoft.Reporting.WinForms.Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report            
            renderedBytes = localReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension); 
            if (format == null)
            {
                return File(renderedBytes, "image/jpeg");
            }
            else if (format == "PDF")
            {
                return File(renderedBytes, "pdf");
            }
            else
            {
                return File(renderedBytes, "image/jpeg");
            }
        }


        public ActionResult DownloadReport(string territory, string format)
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Areas/RDLCReportDemo/RDLC/Report1.rdlc");
            IList<WorldModel> customerList = new List<WorldModel>();
            customerList.Add(new WorldModel("Europe", "SE", "2001", "123"));
            customerList.Add(new WorldModel("Europe", "SE", "2002", "1234"));
            customerList.Add(new WorldModel("Europe", "SE", "2003", "12345"));

            customerList.Add(new WorldModel("Europe", "DE", "2001", "1"));
            customerList.Add(new WorldModel("Europe", "DE", "2002", "12"));
            customerList.Add(new WorldModel("Europe", "DE", "2003", "123"));

            customerList.Add(new WorldModel("Europe", "NE", "2001", "11"));
            customerList.Add(new WorldModel("Europe", "NE", "2002", "112"));
            customerList.Add(new WorldModel("Europe", "NE", "2003", "1123"));

            customerList.Add(new WorldModel("Asia", "IND", "2001", "11"));
            customerList.Add(new WorldModel("Asia", "IND", "2002", "11211"));
            customerList.Add(new WorldModel("Asia", "IND", "2003", "112311"));

            customerList.Add(new WorldModel("Asia", "SYN", "2001", "1121"));
            customerList.Add(new WorldModel("Asia", "SYN", "2002", "112"));
            customerList.Add(new WorldModel("Asia", "SYN", "2003", "11231"));

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "World";
            if (territory != null)
            {
                var customerfilterList = from c in customerList
                    where c.Territory == territory
                    select c;


                reportDataSource.Value = customerfilterList;
            }
            else
                reportDataSource.Value = customerList;

            localReport.DataSources.Add(reportDataSource);
            string reportType = "Image";
            string mimeType;
            string encoding;
            string fileNameExtension;
            //The DeviceInfo settings should be changed based on the reportType            
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx            
            string deviceInfo = "<DeviceInfo>" +
                                "  <OutputFormat>PDF</OutputFormat>" +
                                "  <PageWidth>8.5in</PageWidth>" +
                                "  <PageHeight>11in</PageHeight>" +
                                "  <MarginTop>0.5in</MarginTop>" +
                                "  <MarginLeft>1in</MarginLeft>" +
                                "  <MarginRight>1in</MarginRight>" +
                                "  <MarginBottom>0.5in</MarginBottom>" +
                                "</DeviceInfo>";
            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report            
            renderedBytes = localReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension); 
            if (format == null)
            {
                return File(renderedBytes, "image/jpeg");
            }
            else if (format == "PDF")
            {
                return File(renderedBytes, mimeType);
            }
            else
            {
                return File(renderedBytes, "image/jpeg");
            }
        }
    }
}