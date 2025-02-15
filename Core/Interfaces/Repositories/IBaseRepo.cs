using System.Linq.Expressions;

namespace Core.Interfaces.Repositories;

public interface IBaseRepo<EType>
    where EType : class
{
    Task<EType?> GetByIdAsync(int id);
    Task<IEnumerable<EType>> GetAllAsync(string[]? includes = null);

    // Task<IEnumerable<EType>> GetByConditionAsync(IWhereCondition<EType> condition, IPaginate paginate);
    Task<IEnumerable<EType>> GetByConditionAsync(
        Expression<Func<EType, bool>> exp,
        int offset = 0,
        int limit = 20,
        string[]? includes = null
    );
    Task<EType> AddAsync(EType entity);
    EType Update(EType oldEntity, EType updatedEntity);
    void Delete(EType entity);
    // Task<EType?> FromSqlSingleAsync(string query, params SqlParameter[] parameters);
    // Task<IEnumerable<EType>> FromSqlListAsync(string query, params SqlParameter[] parameters);
}
