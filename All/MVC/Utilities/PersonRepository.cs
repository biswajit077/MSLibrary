using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC.Models;

namespace MVC.Utilities
{
    public interface IPersonRepository : IGenericRepository<Person>
    {
        Person GetSingle(int id);
    }
    public class PersonRepository : GenericRepository<DbContexClass, Person>, IPersonRepository
    {
        public Person GetSingle(int id)
        {
            var query = GetAll().FirstOrDefault(x => x.ID == id);
            var query2 = Context.Persons.FirstOrDefault(x => x.ID == id);

            return query;
        }

        public void Update(Person person)
        {
            Update(person, x => x.ID, x=>x.Address);
            //_repository.Update(obj, o => o.Name, o => o.Description);
            //_repository.Update(obj);
        }
        //using (myDbEntities db = new myDbEntities())
        //{
        //    try
        //    {
        //        //disable detection of changes to improve performance
        //        db.Configuration.AutoDetectChangesEnabled = false;

        //        //for all the entities to update...
        //        MyObjectEntity entityToUpdate = new MyObjectEntity() { Id = 123, Quantity = 100 };
        //        db.MyObjectEntity.Attach(entityToUpdate);

        //        //then perform the update
        //        db.SaveChanges();
        //    }
        //    finally
        //    {
        //        //re-enable detection of changes
        //        db.Configuration.AutoDetectChangesEnabled = true;
        //    }
        //}
    }
}