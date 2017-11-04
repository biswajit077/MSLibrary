using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Data;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcelDataReader;
using LinqToExcel;
using ModelClass;
using MVC.Areas.FileProcessing.Models;
using MVC.Utilities;
using OfficeOpenXml;


namespace MVC.Areas.FileProcessing.Controllers
{
    public class UserController : Controller
    {
        private readonly DbContexClass _db = new DbContexClass();
        #region BulkFileUpload
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase files)
        {
            var listPerson = new List<Person>
            {
                new Person() { PersonID = 1, car = "BMW"},
                new Person() { PersonID = 2, car = "BMW"}
            };

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SomeConnectionString"].ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BatchSize = 100;
                    bulkCopy.DestinationTableName = "dbo.Person";
                    try
                    {
                        bulkCopy.WriteToServer(listPerson.ToDataTable());
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        connection.Close();
                    }
                }

                transaction.Commit();
            }
            return View();
        }
        #endregion
        public FileResult DownloadExcel()
        {
            string path = "/Doc/Users.xlsx";
            return File(path, "application/vnd.ms-excel", "Users.xlsx");
        }

        #region Using OleDb
        [HttpGet]
        public ActionResult FileUploadUsingSqlBulkCopy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FileUploadUsingSqlBulkCopy(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/UploadedFiles/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                conString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.Customers";

                        //[OPTIONAL]: Map the Excel columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("Id", "CustomerId");
                        sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                        sqlBulkCopy.ColumnMappings.Add("Country", "Country");

                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
            }

            return View();
        }
        #endregion

        #region Using EPPLUS

        public ActionResult A(HttpPostedFile Files)
        {
            if (Path.GetExtension(Files.FileName) == ".xlsx")
            {
                using (var excel = new ExcelPackage(Files.InputStream))
                {
                    var tbl = new DataTable();
                    var ws = excel.Workbook.Worksheets.First();
                    var hasHeader = true;  // adjust accordingly
                    // add DataColumns to DataTable
                    foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text
                            : String.Format("Column {0}", firstRowCell.Start.Column));

                    // add DataRows to DataTable
                    int startRow = hasHeader ? 2 : 1;
                    for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        DataRow row = tbl.NewRow();
                        foreach (var cell in wsRow)
                            row[cell.Start.Column - 1] = cell.Text;
                        tbl.Rows.Add(row);
                    }
                    var msg = String.Format("DataTable successfully created from excel-file. Colum-count:{0} Row-count:{1}",
                        tbl.Columns.Count, tbl.Rows.Count);
                    //UploadStatusLabel.Text = msg;
                }
            }
            else
            {
                //UploadStatusLabel.Text = "You did not specify a file to upload.";
            }
            return View();
        }
        #endregion

        #region Using LINQTOEXCEL
        [HttpGet]
        public ActionResult UploadExcelFile()
        {
            return View();
        }
        [HttpPost]
        public JsonResult UploadExcelFile(HttpPostedFileBase FileUpload)
        {
            // Helps: https://github.com/paulyoder/LinqToExcel
            // IF Error: https://www.microsoft.com/en-us/download/confirmation.aspx?id=23734
            List<string> data = new List<string>();
            if (FileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");  
                if (FileUpload.ContentType == "application/vnd.ms-excel" || FileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string filename = FileUpload.FileName;
                    string targetpath = Server.MapPath("~/UploadedFiles/");
                    FileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    string sheetName = "Sheet1";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<UserList>(sheetName) select a;

                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            if (a.Name != "" && a.Address != "" && a.ContactNo != "")
                            {
                                UserList TU = new UserList();
                                TU.Name = a.Name;
                                TU.Address = a.Address;
                                TU.ContactNo = a.ContactNo;
                                //_db.Users.Add(TU);
                                //_db.SaveChanges();
                            }
                            else
                            {
                                data.Add("<ul>");
                                if (a.Name == "" || a.Name == null) data.Add("<li> name is required</li>");
                                if (a.Address == "" || a.Address == null) data.Add("<li> Address is required</li>");
                                if (a.ContactNo == "" || a.ContactNo == null) data.Add("<li>ContactNo is required</li>");

                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {

                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {
                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                                }
                            }
                        }
                    }
                    //deleting excel file from folder  
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //alert message for invalid file format  
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (FileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ExcelDataReader

        public ActionResult ExcelDataReaderDemo()
        {
            using (var stream = System.IO.File.Open("/Doc/Users.xlsx", FileMode.Open, FileAccess.Read))
            {

                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {

                    // Choose one of either 1 or 2:

                    // 1. Use the reader methods
                    do
                    {
                        while (reader.Read())
                        {
                            // reader.GetDouble(0);
                        }
                    } while (reader.NextResult());

                    // 2. Use the AsDataSet extension method
                    //var result = reader.AsDataSet();

                    // The result of each spreadsheet is in result.Tables
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;


                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }

                    //reader.IsFirstRowAsColumnNames = true;

                    //DataSet result = reader.AsDataSet();
                    //var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    //{
                    //    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    //    {
                    //        UseHeaderRow = true
                    //    }
                    //});
                    reader.Close();

                    //return View(result.Tables[0]);
                    return View();
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View();
        }
        #endregion
    }
}