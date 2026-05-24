using System.Linq.Expressions;
using System.Reflection;

namespace PRN232.LMS.Repositories.Common;

public static class QuerySortHelper
{
    public static IQueryable<T> ApplySort<T>(IQueryable<T> query, string? sort, IReadOnlyDictionary<string, string> fieldMap)
    {
        if (string.IsNullOrWhiteSpace(sort))
            return query;

        var isFirst = true;
        foreach (var part in sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var descending = part.StartsWith('-');
            var field = descending ? part[1..] : part;
            var normalized = field.ToLowerInvariant();

            if (!fieldMap.TryGetValue(normalized, out var propertyName))
                continue;

            query = ApplyOrder(query, propertyName, descending, isFirst);
            isFirst = false;
        }

        return query;
    }

    private static IQueryable<T> ApplyOrder<T>(IQueryable<T> query, string propertyName, bool descending, bool isFirst)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(parameter, propertyName);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = isFirst
            ? (descending ? "OrderByDescending" : "OrderBy")
            : (descending ? "ThenByDescending" : "ThenBy");

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), property.Type],
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(resultExpression);
    }
}

