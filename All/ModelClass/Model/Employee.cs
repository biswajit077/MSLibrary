using System.ComponentModel.DataAnnotations;

namespace ModelClass.Model
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Employee")]
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }
    }
}
