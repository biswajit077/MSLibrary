using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelClass.Model
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Company")]
        public string Name { get; set; }
        public List<Employee> Employees { get; set; }

        //public Company()
        //{
        //    Employees = new List<Employee>
        //    {
        //        new Employee{ Name = "Enter name"}
        //    };
        //}
    }
}
