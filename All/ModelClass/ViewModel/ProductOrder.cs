using System.Collections.Generic;

namespace ModelClass.ViewModel
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            Categories = new List<CategoryViewModel>();
        }
        public string Name { get; set; }

        public List<CategoryViewModel> Categories { get; set; }
    }

    public class CategoryViewModel
    {
        public CategoryViewModel()
        {
            Subcategories = new List<SubCategoryViewModel>();
        }
        public string Name { get; set; }
        public List<SubCategoryViewModel> Subcategories { get; set; }
    }

    public class SubCategoryViewModel
    {
        public SubCategoryViewModel()
        {
            Products = new List<ProductViewModel>();
        }
        public string Name { get; set; }
        public List<ProductViewModel> Products { get; set; }
        public string PreviousFieldId { get; set; }
    }

    public class ProductViewModel
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string PreviousFieldId { get; set; }
    }
}
