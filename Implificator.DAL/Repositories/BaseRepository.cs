using Implificator.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implificator.DAL.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : Entity
    {

        private readonly UserContext _context;

        public BaseRepository(UserContext context)
        {
            _context = context;
        }

        public async Task Add(TEntity entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TEntity> GetAll()
            => _context.Set<TEntity>();

        public async Task<TEntity> GetById(int id)
            => await _context.Set<TEntity>()
                             .SingleOrDefaultAsync(s => s.Id == id);

        public async Task Remove(TEntity entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TEntity entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
