using Implificator.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implificator.DAL.Repositories
{
    public interface IBaseRepository<T>
    {
        public Task Add(T entity);
        public Task Remove(T entity);
        public Task<T> GetById(int id);
        public IQueryable<T> GetAll();
        public Task Update(T entity);
    }
}
