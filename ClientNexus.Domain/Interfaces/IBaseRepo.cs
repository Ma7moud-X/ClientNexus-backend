using System.Linq.Expressions;

namespace ClientNexus.Domain.Interfaces;

public interface IBaseRepo<EType>
    where EType : class
{
    Task<EType?> GetByIdAsync(int id);
    Task<IEnumerable<EType>> GetAllAsync(string[]? includes = null);

    public IQueryable<EType> GetAllQueryable(params Expression<Func<EType, object>>[] includes);

    // Task<IEnumerable<EType>> GetByConditionAsync(IWhereCondition<EType> condition, IPaginate paginate);
    Task<IEnumerable<EType>> GetByConditionAsync(
        Expression<Func<EType, bool>> condExp,
        bool getAll = false,
        int offset = 0,
        int limit = 20,
        string[]? includes = null
    );
    Task<IEnumerable<T>> GetByConditionAsync<T>(
        Expression<Func<EType, bool>>? condExp,
        Expression<Func<EType, T>> selectExp,
        bool getAll = false,
        int offset = 0,
        int limit = 20
    );

    Task<IEnumerable<T>> GetByConditionAsync<T>(
        IEnumerable<(string conditionString, object conditionValue)> conditions,
        Expression<Func<EType, T>> selectExp,
        bool getAll = false,
        int offset = 0,
        int limit = 20
    );

    Task<EType> AddAsync(EType entity);
    Task<EType?> FirstOrDefaultAsync(
        Expression<Func<EType, bool>> condExp,
        Func<IQueryable<EType>, IQueryable<EType>>? include = null
    );
    Task<int> CountAsync(Expression<Func<EType, bool>>? predicate = null);
    EType Update(EType oldEntity, EType updatedEntity);
    void Delete(EType entity);
    Task<bool> CheckAnyExistsAsync(Expression<Func<EType, bool>> condExp);
    // Task<EType?> FromSqlSingleAsync(string query, params SqlParameter[] parameters);
    // Task<IEnumerable<EType>> FromSqlListAsync(string query, params SqlParameter[] parameters);
}
