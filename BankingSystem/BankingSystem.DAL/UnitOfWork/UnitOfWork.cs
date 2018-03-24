using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Linq;
using BankingSystem.DAL.Interfaces;
using BankingSystem.DAL.DbContexts;
using BankingSystem.DAL.DomainModels;

namespace BankingSystem.DAL.UnitOfWork
{
    public partial class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContext _dbContext;
        private bool _disposed;

        public UnitOfWork(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public UnitOfWork()
        {
            _dbContext = new BankingSystemDb();
        }

        public int CommitChanges()
        {
            return _dbContext.CommitChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this._disposed = true;
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
