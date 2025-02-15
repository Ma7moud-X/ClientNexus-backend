using System.Linq.Expressions;

namespace Core.Interfaces.Filters;

public interface IWhereCondition<EType>
    where EType : class
{
    Expression<Func<EType, bool>> ToWhereCondition();
}
