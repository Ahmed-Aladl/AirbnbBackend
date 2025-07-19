using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        public AirbnbContext Db { get; set; }
        public Repository(AirbnbContext _db)
        {
            Db = _db;
        }

        public virtual List<TEntity> GetAll()
        {
            return Db.Set<TEntity>().ToList();
        }

        public virtual TEntity GetById(TKey id)
        {
            return Db.Set<TEntity>().Find(id);

        }

        public virtual void Add(TEntity entity)
        {
            Db.Set<TEntity>().Add(entity);
        }
        public virtual void AddRange(ICollection<TEntity> entities)
        {
            Db.Set<TEntity>().AddRange(entities);
        }
        public virtual void Update(TEntity entity)
        {
            Db.Entry<TEntity>(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
        public virtual void Delete(TEntity entity)
        {
            Db.Set<TEntity>().Remove(entity);
        }


        //public async Task<TEntity?> GetByConditionAsync(
        //    Expression<Func<TEntity, bool>> predicate,
        //    Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        //{
        //    var query = Db.Set<TEntity>().AsQueryable();
        //    if (include != null)
        //        query = include(query);

        //    return await query.FirstOrDefaultAsync(predicate);
        //}
        

        //public List<TEntity> GetPage<TOrder>(Expression<Func<TEntity, bool>> filterExp, Expression<Func<TEntity, TOrder>> orderExp, string search = "", int page = 1, int pageSize = 10) where TOrder : IComparable<TOrder>
        //{
        //    Console.WriteLine("********************************************************************\nBegin");
        //    IQueryable<TEntity> result = Db.Set<TEntity>().AsNoTracking();

        //    if (!string.IsNullOrEmpty(search))
        //        result = result.Where(filterExp);
        //    int courseCount = result.Count();

        //    result = result.OrderBy(orderExp)
        //                    .Skip((page - 1) * pageSize)
        //                    .Take(pageSize);



        //    return result.ToList();
        //}


    }
}
