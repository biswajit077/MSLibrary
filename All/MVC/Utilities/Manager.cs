using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

using MVC.Models;
using MVC.Utilities;

namespace MVC.Utilities
{
    public class Manager
    {
        private readonly DbContexClass _db;
        public Manager()
        {
            _db = new DbContexClass();
        }

        public IList<Person> GetPersons()
        {
            return _db.Persons.ToList();
        }
        public void InsertOrUpdate(Person person)
        {
            using (_db)
            {
                _db.Entry(person).State = person.ID == 0 ?
                    EntityState.Added :
                    EntityState.Modified;

                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Single Data or Single Row AddOrUpdate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity AddOrUpdate<TEntity>(DbContext context, TEntity entity)
            where TEntity : class
        {
            var tracked = context.Set<TEntity>().Find(context.KeyValuesFor(entity));
            if (tracked != null)
            {
                context.Entry(tracked).CurrentValues.SetValues(entity);
                return tracked;
            }

            context.Set<TEntity>().Add(entity);
            return entity;
        }
        public void UpdatePerson(Person person)
        {
            this._db.Persons.Attach(person);
            DbEntityEntry<Person> entry = _db.Entry(person);
            entry.State = EntityState.Modified;
            _db.SaveChanges();

            //var original = _db.Persons.Find(person.ID);

            //if (original != null)
            //{
            //    _db.Entry(original).CurrentValues.SetValues(person);
            //    _db.SaveChanges();
            //}
        }
        public void UpdatePersonNameOnly(Person person)
        {
            this._db.Persons.Attach(person);
            DbEntityEntry<Person> entry = _db.Entry(person);
            entry.Property(e => e.Name).IsModified = true;
            _db.SaveChanges();
        }
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        public static void SetPropertyValue(object obj, string propName, object value)
        {
            obj.GetType().GetProperty(propName).SetValue(obj, value, null);
        }
        public static void CopyIfDifferent(Object target, Object source)
        {
            foreach (var prop in target.GetType().GetProperties())
            {
                var targetValue = GetPropValue(target, prop.Name);
                var sourceValue = GetPropValue(source, prop.Name);
                if (!targetValue.Equals(sourceValue))
                {
                    SetPropertyValue(target, prop.Name, sourceValue);
                }
            }
        }

        static long DirectorySize(DirectoryInfo dInfo, bool includeSubDir)
        {
            // Enumerate all the files
            long totalSize = dInfo.EnumerateFiles()
                .Sum(file => file.Length);

            // If Subdirectories are to be included
            if (includeSubDir)
            {
                // Enumerate all sub-directories
                totalSize += dInfo.EnumerateDirectories()
                    .Sum(dir => DirectorySize(dir, true));
            }
            return totalSize;
        }
    }

    public class EFRepository<T> : IRepository<T> where T : class
    {
        protected DbContext DbContext { get; set; }
        protected DbSet<T> DbSet { get; set; }

        public EFRepository(DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");
            DbContext = dbContext;
            DbSet = DbContext.Set<T>();
        }

        public virtual void Add(T entity)
        {
            DbContext.Entry(entity).State = EntityState.Added;
        }

        public virtual void Update(T entity)
        {
            //dbEntityEntry.State = EntityState.Modified; --- I cannot do this.

            //Ensure only modified fields are updated.
            var dbEntityEntry = DbContext.Entry(entity);
            foreach (var property in dbEntityEntry.OriginalValues.PropertyNames)
            {
                var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
                var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
                if (original != null && !original.Equals(current))
                    dbEntityEntry.Property(property).IsModified = true;
            }
        }

        public virtual void Update(T entity, params Expression<Func<T, object>>[] updatedProperties)
        {
            //dbEntityEntry.State = EntityState.Modified; --- I cannot do this.

            //Ensure only modified fields are updated.
            var dbEntityEntry = DbContext.Entry(entity);
            if (updatedProperties.Any())
            {
                //update explicitly mentioned properties
                foreach (var property in updatedProperties)
                {
                    dbEntityEntry.Property(property).IsModified = true;
                }
            }
            else
            {
                //no items mentioned, so find out the updated entries
                foreach (var property in dbEntityEntry.OriginalValues.PropertyNames)
                {
                    var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
                    var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
                    if (original != null && !original.Equals(current))
                        dbEntityEntry.Property(property).IsModified = true;
                }
            }
        }
        //_repository.Update(obj);
        //_repository.Update(obj, o => o.Name, o => o.Description);
    }

    public interface IRepository<T>
    {
    }
}