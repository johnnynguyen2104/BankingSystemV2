using BankingSystem.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.DAL.Interfaces
{
    public interface IRepository<TKey, TEntity> where TEntity : BaseEntity<TKey>
        where TKey : struct
    {
        TEntity Create(TEntity entity);

        IQueryable<TEntity> Read(Expression<Func<TEntity, bool>> expression);

        TEntity ReadOne(Expression<Func<TEntity, bool>> expression);

        bool Update(TEntity entity);

        bool Delete(Expression<Func<TEntity, bool>> expression);

        bool Delete(TEntity entity);

        void UpdateValues(TEntity entity, string propertyName, string value);

        Task<TEntity> CreateAsyn(TEntity entity);

        Task<IQueryable<TEntity>> ReadAsyn(Expression<Func<TEntity, bool>> expression);

        Task<TEntity> ReadOneAsyn(Expression<Func<TEntity, bool>> expression);

        Task<bool> UpdateAsyn(TEntity entity);

        Task<bool> DeleteAsyn(Expression<Func<TEntity, bool>> expression);

        Task<bool> DeleteAsyn(TEntity entity);

        Task UpdateValuesAsyn(TEntity entity, string propertyName, string value);
    }
}
