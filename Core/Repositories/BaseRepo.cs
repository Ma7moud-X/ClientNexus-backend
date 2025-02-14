using Core.Interfaces.Repositories;
using Database;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories;

public class BaseRepo<EType> : IBaseRepo<EType>
    where EType : class
{
    private readonly ApplicationDbContext _context;

    public BaseRepo(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EType> AddAsync(EType entity)
    {
        if (entity is null)
        {
            throw new Exception("Can't add a null entity");
        }

        await _context.Set<EType>().AddAsync(entity);
        return entity;
    }

    public void Delete(EType entity)
    {
        if (entity is null)
        {
            throw new Exception("Can't delete a null entity");
        }

        _context.Set<EType>().Remove(entity);
    }

    public async Task<IEnumerable<EType>> GetAllAsync(string[]? includes = null)
    {
        IQueryable<EType> query = _context.Set<EType>().AsNoTracking();
        if (includes is not null)
        {
            foreach (string inc in includes)
            {
                query = query.Include(inc);
            }
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<EType>> GetByConditionAsync(
        Func<EType, bool> exp,
        int offset = 0,
        int limit = 20,
        string[]? includes = null
    )
    {
        IQueryable<EType> query = _context
            .Set<EType>()
            .AsNoTracking()
            .Where(exp)
            .Skip(offset)
            .Take(limit)
            .AsQueryable();

        if (includes is not null)
        {
            foreach (var inc in includes)
            {
                query = query.Include(inc);
            }
        }

        return await query.ToListAsync();
    }

    public async Task<EType?> GetByIdAsync(int id)
    {
        return await _context.Set<EType>().FindAsync(id);
    }

    public EType Update(EType oldEntity, EType updatedEntity)
    {
        _context.Entry<EType>(oldEntity).CurrentValues.SetValues(updatedEntity);
        return updatedEntity;
    }
}
