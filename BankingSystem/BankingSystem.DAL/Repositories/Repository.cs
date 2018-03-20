using BankingSystem.DAL.DomainModels;
using BankingSystem.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.DAL.Repositories
{
    public class Repository<TKey, TEntity> : IRepository<TKey, TEntity> where TKey : struct
        where TEntity : BaseEntity<TKey>
    {
        private readonly IDbContext _dbContext;

        public Repository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TEntity Create(TEntity entity)
        {
            return entity == null ? null : _dbContext.Set<TEntity>().Add(entity);
        }

        public bool Delete(TEntity entity)
        {
            if (entity == null) throw  new ArgumentNullException("entity", "Entity can't be null.");
            _dbContext.Entry(entity).State = EntityState.Deleted;
            return true;
        }

        public void UpdateValues(TEntity entity, string propertyName, string value)
        {
            Type sourceType = typeof(TEntity);
            var properties = sourceType.GetProperties();
            foreach (PropertyInfo propInfo in properties)
            {
                if (propInfo.Name == propertyName)
                {
                    propInfo.SetValue(entity, ChangeType<int?>(value), null);
                    return;
                }
            }
        }

        public bool Delete(Expression<Func<TEntity, bool>> expression)
        {
            if (expression == null)
            {
                throw  new ArgumentNullException("expression", "Expression can't be Null.");
            }

            var deletedList = _dbContext.Set<TEntity>().Where(expression).ToList();

            foreach (var item in deletedList)
            {
                _dbContext.Entry(item).State = EntityState.Deleted;
            }
            return true;
        }

        public IQueryable<TEntity> Read(Expression<Func<TEntity, bool>> expression)
        {
            return _dbContext.Set<TEntity>().Where(expression);
        }

        public TEntity ReadOne(Expression<Func<TEntity, bool>> expression)
        {
            return _dbContext.Set<TEntity>().FirstOrDefault(expression);
        }

        public bool Update(TEntity entity)
        {
            if (entity == null)
            {
                throw  new ArgumentNullException("entity", "Entity can't be null");
            }

            _dbContext.Entry(entity).State = EntityState.Modified;

            return true;
        }

        private T ChangeType<T>(object value)
        {
            var t = typeof(T);

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null)
                {
                    return default(T);
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return (T)Convert.ChangeType(value, t);
        }

        public Task<TEntity> CreateAsyn(TEntity entity)
        {
            return new Task<TEntity>(() =>
            {
                return Create(entity);
            });
        }

        public Task<IQueryable<TEntity>> ReadAsyn(Expression<Func<TEntity, bool>> expression)
        {
            return new Task<IQueryable<TEntity>>(() => {
                return Read(expression);
            });
        }

        public Task<TEntity> ReadOneAsyn(Expression<Func<TEntity, bool>> expression)
        {
            return new Task<TEntity>(() => {
                return ReadOne(expression);
            });
        }

        public Task<bool> UpdateAsyn(TEntity entity)
        {
            return new Task<bool>(() => {
                return Update(entity);
            });
        }

        public Task<bool> DeleteAsyn(Expression<Func<TEntity, bool>> expression)
        {
            return new Task<bool>(() => {
                return Delete(expression);
            }); 
        }

        public Task<bool> DeleteAsyn(TEntity entity)
        {
            return new Task<bool>(() => {

                return Delete(entity);
            });
        }

        public Task UpdateValuesAsyn(TEntity entity, string propertyName, string value)
        {
            return new Task(() => { UpdateValues(entity, propertyName, value); });
        }
    }
}
