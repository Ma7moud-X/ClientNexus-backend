using System.Linq.Expressions;
using ClientNexus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClientNexus.Infrastructure.Repositories;

public class BaseRepo<EType> : IBaseRepo<EType>
    where EType : class
{
    private readonly ApplicationDbContext _context;

    public BaseRepo(ApplicationDbContext context)
    {
        if (context is null)
        {
            throw new Exception("Can't initialize BaseRepo. Context can't be null");
        }

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

    // public async Task<IEnumerable<EType>> FromSqlListAsync(string query, params SqlParameter[] parameters)
    // {
    //     return await _context.Set<EType>().FromSqlRaw(query, parameters).ToListAsync();
    // }

    // public async Task<EType?> FromSqlSingleAsync(string query, params SqlParameter[] parameters)
    // {
    //     return await _context.Set<EType>().FromSqlRaw(query, parameters).FirstOrDefaultAsync();
    // }


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
        Expression<Func<EType, bool>> condExp,
        bool getAll = false,
        int offset = 0,
        int limit = 20,
        string[]? includes = null
    )
    {
        if (condExp is null)
        {
            throw new Exception("Expression can't be null");
        }

        var query = _context.Set<EType>().AsNoTracking().Where(condExp).AsQueryable();

        if (!getAll)
        {
            query = query.Skip(offset).Take(limit);
        }

        if (includes is not null)
        {
            foreach (var inc in includes)
            {
                query = query.Include(inc);
            }
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<object>> GetByConditionAsync(
        Expression<Func<EType, bool>>? condExp,
        Expression<Func<EType, object>> selectExp,
        bool getAll = false,
        int offset = 0,
        int limit = 20
    )
    {
        var q = _context.Set<EType>().AsNoTracking();
        if (condExp is not null)
        {
            q = q.Where(condExp);
        }

        if (selectExp is null)
        {
            throw new Exception("Select expression can't be null");
        }

        IQueryable<object> query = q.Select(selectExp);
        if (!getAll)
        {
            query = query.Skip(offset).Take(limit);
        }

        return await query.ToListAsync();
    }
    public async Task<EType?> FirstOrDefaultAsync(Expression<Func<EType, bool>> condExp,Func<IQueryable<EType>, IQueryable<EType>>? include = null)
    {
        IQueryable<EType> q = _context.Set<EType>().AsNoTracking();

        if (include != null)
        {
            q = include(q);
        }

        return await q.FirstOrDefaultAsync(condExp);
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
