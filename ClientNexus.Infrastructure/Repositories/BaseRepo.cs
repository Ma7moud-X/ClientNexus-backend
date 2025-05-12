using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
using ClientNexus.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Amazon.S3.Util.S3EventNotification;

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

    public IQueryable<EType> GetAllQueryable(params Expression<Func<EType, object>>[] includes)
    {
        IQueryable<EType> query = _context.Set<EType>().AsNoTracking();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
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
            if (offset < 0)
            {
                throw new InvalidInputException(
                    $"{nameof(offset)} must be greater than or equal zero"
                );
            }

            if (limit <= 0)
            {
                throw new InvalidInputException($"{nameof(limit)} must be greater than 0");
            }

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

    public async Task<IEnumerable<T>> GetByConditionAsync<T>(
        Expression<Func<EType, bool>>? condExp,
        Expression<Func<EType, T>> selectExp,
        bool getAll = false,
        int offset = 0,
        int limit = 20,
        Expression<Func<EType, object>>? orderByExp = null,
        bool descendingOrdering = false
    )
    {
        if (selectExp is null)
        {
            throw new Exception("Select expression can't be null");
        }

        var q = _context.Set<EType>().AsNoTracking();
        if (condExp is not null)
        {
            q = q.Where(condExp);
        }

        if (orderByExp is not null)
        {
            if (descendingOrdering)
            {
                q = q.OrderByDescending(orderByExp);
            }
            else
            {
                q = q.OrderBy(orderByExp);
            }
        }

        IQueryable<T> query = q.Select(selectExp);
        if (!getAll)
        {
            if (offset < 0)
            {
                throw new InvalidInputException(
                    $"{nameof(offset)} must be greater than or equal zero"
                );
            }

            if (limit <= 0)
            {
                throw new InvalidInputException($"{nameof(limit)} must be greater than 0");
            }

            query = query.Skip(offset).Take(limit);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetByConditionAsync<T>(
        IEnumerable<(string conditionString, object conditionValue)> conditions,
        Expression<Func<EType, T>> selectExp,
        bool getAll = false,
        int offset = 0,
        int limit = 20,
        Expression<Func<EType, object>>? orderByExp = null,
        bool descendingOrdering = false
    )
    {
        ArgumentNullException.ThrowIfNull(conditions);
        ArgumentNullException.ThrowIfNull(selectExp);

        var q = _context.Set<EType>().AsNoTracking();
        foreach (var condition in conditions)
        {
            if (string.IsNullOrEmpty(condition.conditionString) || condition.conditionValue is null)
            {
                continue;
            }

            q = q.Where(condition.conditionString, condition.conditionValue);
        }

        if (orderByExp is not null)
        {
            if (descendingOrdering)
            {
                q = q.OrderByDescending(orderByExp);
            }
            else
            {
                q = q.OrderBy(orderByExp);
            }
        }

        if (selectExp is null)
        {
            throw new Exception("Select expression can't be null");
        }

        IQueryable<T> query = q.Select(selectExp);
        if (!getAll)
        {
            if (offset < 0)
            {
                throw new InvalidInputException(
                    $"{nameof(offset)} must be greater than or equal zero"
                );
            }

            if (limit <= 0)
            {
                throw new InvalidInputException($"{nameof(limit)} must be greater than 0");
            }

            query = query.Skip(offset).Take(limit);
        }

        return await query.ToListAsync();
    }

    public async Task<EType?> FirstOrDefaultAsync(
        Expression<Func<EType, bool>> condExp,
        Func<IQueryable<EType>, IQueryable<EType>>? include = null
    )
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

    public async Task<EType?> GetByIdWithLockAsync(int id)
    {
        var entityType = _context.Model.FindEntityType(typeof(EType));
        if (entityType == null)
            throw new InvalidOperationException(
                $"Entity type '{typeof(EType).Name}' is not part of the EF Core model. "
                    + "Ensure it is added as a DbSet or configured in OnModelCreating."
            );

        var tableName = entityType.GetSchema() is string schema
            ? $"[{schema}].[{entityType.GetTableName()}]"
            : $"[{entityType.GetTableName()}]";

        var sql = $"SELECT * FROM {tableName} WITH (UPDLOCK, ROWLOCK) WHERE Id = @id";

        var param = new SqlParameter("@id", id);

        // To suppress the SQL injection warning safely, use raw string, not interpolation
        return await _context.Set<EType>().FromSqlRaw(sql, param).FirstOrDefaultAsync();
    }

    public async Task<int> CountAsync(Expression<Func<EType, bool>>? predicate = null)
    {
        if (predicate == null)
        {
            return await _context.Set<EType>().CountAsync();
        }

        return await _context.Set<EType>().Where(predicate).CountAsync();
    }

    public EType Update(EType oldEntity, EType updatedEntity)
    {
        var entry = _context.Entry<EType>(oldEntity);
        if (entry.State == EntityState.Detached)
        {
            _context.Set<EType>().Attach(oldEntity);
        }
        entry.CurrentValues.SetValues(updatedEntity);
        entry.State = EntityState.Modified;
        return updatedEntity;
    }

    public void Update(EType entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public async Task<bool> CheckAnyExistsAsync(Expression<Func<EType, bool>> condExp)
    {
        return await _context.Set<EType>().AnyAsync(condExp);
    }
}
