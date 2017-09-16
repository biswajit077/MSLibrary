using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MVC.Models;

namespace MVC.Utilities
{
    public class DbContexClass:DbContext
    {
        public DbContexClass():base("name=DbConnection")
        {
        }
        public DbSet<Person> Persons { get; set; }
    }
}