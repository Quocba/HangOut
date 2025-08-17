using System.Linq.Expressions;

namespace HangOut.Domain.Filter;

public interface IFilter<T>
{
    Expression<Func<T, bool>> ToExpression();
}