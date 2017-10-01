using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ModelClass.ViewModel;

namespace MVC.Areas.DymaicTemplates.Controllers
{
    public class OrderManagerController : Controller
    {
        // GET: DymaicTemplates/OrderManager
        public ActionResult Index()
        {
            return View();
        }

        // GET: /OrderManager/Edit
        public ActionResult Edit()
        {
            OrderViewModel viewModel = new OrderViewModel();
            viewModel.Name = "Order 1";
            for (int i = 0; i < 1; i++)
            {
                var category = new CategoryViewModel();
                category.Name = "category " + i;
                for (int j = 0; j < 2; j++)
                {
                    var subCategory = new SubCategoryViewModel();
                    subCategory.Name = "SubCategory " + j;
                    for (int k = 0; k < 2; k++)
                    {
                        var product = new ProductViewModel();
                        product.Name = "Product " + k;
                        subCategory.Products.Add(product);
                    }
                    category.Subcategories.Add(subCategory);
                }

                viewModel.Categories.Add(category);
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(OrderViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                //TODO: save the data
            }
            return View(viewModel);
        }

        public ActionResult AddCategory()
        {
            return PartialView("CategoryPartial", new CategoryViewModel());
        }

        [HttpPost]
        public ActionResult AddSubCategory(string id)
        {
            return PartialView("SubcategoryPartial", new SubCategoryViewModel() { PreviousFieldId = id });
        }

        [HttpPost]
        public ActionResult AddProduct(string id)
        {
            return PartialView("ProductPartial", new ProductViewModel() { PreviousFieldId = id });
        }
    }
}