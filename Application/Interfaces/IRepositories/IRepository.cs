using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IRepositories
{
    public interface IRepository<T, TKey>
        where T : class
    {
        List<T> GetAll();
        T GetById(TKey id);

        void Add(T entity);
        void AddRange(ICollection<T> entities);
        void Update(T entity);
        void Delete(T entity);
    }
}
