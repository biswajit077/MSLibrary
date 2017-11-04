using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace MVC.Utilities
{
    public interface IUnitOfWork : IDisposable
    {
        //ICategoryRepository Categories { get; }
        //IProductRepository Products { get; }

        int SaveChanges();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContexClass db;
        private readonly DbContext _db;
        private bool _disposed;
        public UnitOfWork()
        {
            db = new DbContexClass();
        }
        public UnitOfWork(DbContext context)
        {
            _db = context;
        }
        //private ICategoryRepository _Categories;
        //public ICategoryRepository Categories
        //{
        //    get
        //    {
        //        if (this._Categories == null)
        //        {
        //            this._Categories = new CategoryRepository(db);
        //        }
        //        return this._Categories;
        //    }
        //}

        //private IProductRepository _Products;
        //public IProductRepository Products
        //{
        //    get
        //    {
        //        if (this._Products == null)
        //        {
        //            this._Products = new ProductRepository(db);
        //        }
        //        return this._Products;
        //    }
        //}

        //public int SaveChanges()
        //{
        //    return db.SaveChanges();
        //}
        //public void Dispose()
        //{
        //    db.Dispose();
        //}
        public int SaveChanges()
        {
            return ((IObjectContextAdapter)db).ObjectContext.SaveChanges();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, 
        ///     releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Disposes off the managed and unmanaged resources used.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_disposed)
                return;

            _disposed = true;
        }

        private bool disposed = false;

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!this.disposed)
        //    {
        //        if (disposing)
        //        {
        //            entities.Dispose();
        //        }
        //    }
        public void Rollback()
        {
            _db
                .ChangeTracker
                .Entries()
                .ToList()
                .ForEach(x => x.Reload());
        }
        /// <summary>
        /// Disposes all external resources.
        /// </summary>
        /// <param name="disposing">The dispose indicator.</param>
        //private void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (_dbContext != null)
        //        {
        //            _dbContext.Dispose();
        //            _dbContext = null;
        //        }
        //    }
        //}
    }
}