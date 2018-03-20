using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using BankingSystem.DAL.DomainModels;

namespace BankingSystem.DAL.Interfaces
{
    public interface IDbContext
    {
        int CommitChanges();

        Task<int> CommitChangesAsyn();

        DbSet<TEntity> Set<TEntity>() where TEntity : class ;

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class ;

        void Dispose();
    }
}
