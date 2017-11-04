using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;


namespace MVC.Utilities
{
    public interface IGenericRepository<T>:IDisposable where T : class
    {
        IUnitOfWork UnitOfWork { get; }
        /// <summary>
        ///     Counts the specified entities.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <returns></returns>
        int Count();
        /// <summary>
        ///     Counts entities with the specified criteria.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        //int Count(Expression<Func<T, bool>> criteria = null);
        //public IEnumerable<T> GetAll(Func<T, bool> predicate = null)
        //{
        //    if (predicate != null)
        //    {
        //        return _objectSet.Where(predicate);
        //    }

        //    return _objectSet.AsEnumerable();
        //}
        int Count(Expression<Func<T, bool>> criteria);
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void AddOrEdit(T entity);
        void Edit(T entity);
        void Update(T entity);
        void Update(T entity, params Expression<Func<T, object>>[] updatedProperties);
        void UdtadeMultipleRows(IEnumerable<T> entities);
        void Delete(T entity);
        void Delete(object Id);
        void DeleteRange(IEnumerable<T> entities);
        void Delete(Expression<Func<T, bool>> criteria);

        void Save();

    }

    public abstract class GenericRepository<C, T> :
        IGenericRepository<T> where T : class where C : DbContext, new()
    {
        private C _entities = new C();
        private IUnitOfWork unitOfWork;
        private bool bDisposed;

        public C Context
        {
            get { return _entities; }
            set { _entities = value; }
        }

        //Unit of Work Property
        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (UnitOfWork == null)
                {
                    unitOfWork = new UnitOfWork(_entities);
                }
                return unitOfWork;
            }
        }
        public virtual int Count()
        {
            return _entities.Set<T>().Count();
        }

        public virtual int Count (Expression<Func<T, bool>> criteria)
        {
            return _entities.Set<T>().Count(criteria);
        }
        public virtual IQueryable<T> GetAll()
        {
            IQueryable<T> query = _entities.Set<T>();
            return query;
        }

        public IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _entities.Set<T>().Where(predicate);
            return query;
        }

        public virtual void Add(T entity)
        {
            _entities.Set<T>().Add(entity);
        }
        public virtual void AddRange(IEnumerable<T> entities)
        {
            _entities.Set<T>().AddRange(entities);
        }
        public virtual void AddOrEdit(T entity)
        {
            _entities.Set<T>().AddOrUpdate(entity);
        }
        public virtual void Delete(T entity)
        {
            _entities.Set<T>().Remove(entity);
        }
        public virtual void Delete(object Id)
        {
            T entity = _entities.Set<T>().Find(Id);
            if (entity != null) _entities.Set<T>().Remove(entity);
        }
        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            _entities.Set<T>().RemoveRange(entities);
        }

        public virtual void Delete(Expression<Func<T, bool>> criteria)
        {
            IQueryable<T> listQueryable = _entities.Set<T>().Find(criteria) as IQueryable<T>;
            if (listQueryable != null) _entities.Set<T>().RemoveRange(listQueryable);
        }
        public virtual void Edit(T entity)
        {
            _entities.Entry(entity).State = EntityState.Modified;
        }
        public virtual void Update(T entity)
        {
            _entities.Set<T>().Attach(entity);
        }
        public virtual void UdtadeMultipleRows(IEnumerable<T> entities)
        {
            using (C dc = new C())
            {
                foreach (var item in entities)
                {

                    //var c = dc.Set<T>().Where(a => a.ContactID.Equals(i.ContactID)).FirstOrDefault();
                    //if (c != null)
                    //{
                    //    c.ContactPerson = i.ContactPerson;
                    //    c.Contactno = i.Contactno;
                    //    c.EmailID = i.EmailID;
                    //}
                }
                dc.SaveChanges();
            }
        }
        public virtual void Update(T entity, params Expression<Func<T, object>>[] updatedProperties)
        { 
            //Ensure only modified fields are updated.
            var dbEntityEntry = _entities.Entry(entity);
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
        public virtual void Save()
        {
            _entities.SaveChanges();
        }

        public void Dispose()
        {
            this.Close();
        }

        private string RemoveAccent(string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        private bool IsValidTag(string tag, string tags)
        {
            string[] allowedTags = tags.Split(',');
            if (tag.IndexOf("javascript", StringComparison.Ordinal) >= 0) return false;
            if (tag.IndexOf("vbscript", StringComparison.Ordinal) >= 0) return false;
            if (tag.IndexOf("onclick", StringComparison.Ordinal) >= 0) return false;

            var endchars = new char[] { ' ', '>', '/', '\t' };

            int pos = tag.IndexOfAny(endchars, 1);
            if (pos > 0) tag = tag.Substring(0, pos);
            if (tag[0] == '/') tag = tag.Substring(1);

            foreach (string aTag in allowedTags)
            {
                if (tag == aTag) return true;
            }

            return false;
        }

        #region Disposing Methods

        protected void Dispose(bool bDisposing)
        {
            if (!bDisposed)
            {
                if (bDisposing)
                {
                    if (null != _entities)
                    {
                        _entities.Dispose();
                    }
                }
                bDisposed = true;
            }
        }

        public void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public int ExecuteCommand(string sqlCommand, params object[] parameters)
        {
            return _entities.Database.ExecuteSqlCommand(sqlCommand, parameters);
        }
        public IEnumerable<T> ExecStoreProcedure<T>(string sql, params object[] parameters)
        {
            return _entities.Database.SqlQuery<T>(sql, parameters);
        }
        public IEnumerable<T> ExecWithStoreProcedure(string query, params object[] parameters)
        {
            return _entities.Database.SqlQuery<T>(query, parameters).ToList();
        }
        public IEnumerable<T> SQLQueryList<T>(string sql)
        {
            return _entities.Database.SqlQuery<T>(sql);
        }
        public T SQLQuery<T>(string sql)
        {
            
            return _entities.Database.SqlQuery<T>(sql).FirstOrDefault();
        }

    }
}