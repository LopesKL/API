using System;
using System.Linq;
using System.Linq.Expressions;

namespace Application
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> source, string sortField, string sortOrder)
        {
            // Converte `sortField` para PascalCase para combinar com as propriedades do modelo
            var propertyName = char.ToUpper(sortField[0]) + sortField.Substring(1);

            // Obtém a propriedade correspondente pelo nome PascalCase
            var param = Expression.Parameter(typeof(T), "x");
            var property = typeof(T).GetProperty(propertyName);

            // Se a propriedade não for encontrada, lança uma exceção
            if (property == null)
                throw new ArgumentException($"Property '{propertyName}' does not exist on type '{typeof(T)}'");

            // Cria a expressão de acesso à propriedade
            var propertyAccess = Expression.MakeMemberAccess(param, property);
            var orderByExpression = Expression.Lambda(propertyAccess, param);

            // Define o método de ordenação dinamicamente ("OrderBy" ou "OrderByDescending")
            var orderByMethod = sortOrder.ToLower() == "ascend" || sortOrder.ToLower() == "asc"
                ? "OrderBy"
                : "OrderByDescending";

            // Cria a expressão de chamada para OrderBy/OrderByDescending
            var resultExpression = Expression.Call(typeof(Queryable), orderByMethod,
                new Type[] { typeof(T), property.PropertyType },
                source.Expression, Expression.Quote(orderByExpression));

            return source.Provider.CreateQuery<T>(resultExpression);
        }

        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> source, object filter)
        {
            // Obtém as propriedades do filtro (RequestAllCrudDto) e as propriedades da entidade (T)
            var filterProperties = filter.GetType().GetProperties();
            var entityProperties = typeof(T).GetProperties().ToDictionary(p => p.Name, p => p);

            foreach (var property in filterProperties)
            {
                // Verifica se a propriedade do filtro está presente na entidade
                if (entityProperties.ContainsKey(property.Name))
                {
                    var value = property.GetValue(filter);

                    // Aplica o filtro apenas se o valor for válido
                    if (value != null && !string.IsNullOrEmpty(value.ToString()))
                    {
                        var parameter = Expression.Parameter(typeof(T), "x");
                        var member = Expression.Property(parameter, entityProperties[property.Name]);

                        // Verifica se a propriedade é uma string para usar Contains
                        Expression condition;
                        if (member.Type == typeof(string))
                        {
                            // Cria a expressão para Contains: x => x.Property.Contains(value)
                            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            var constant = Expression.Constant(value, typeof(string));
                            condition = Expression.Call(member, containsMethod, constant);
                        }
                        else
                        {
                            // Para outros tipos, usa Equal: x => x.Property == value
                            var constant = Expression.Constant(value);
                            condition = Expression.Equal(member, constant);
                        }

                        // Cria a expressão lambda para o filtro
                        var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

                        // Aplica o filtro ao IQueryable
                        source = source.Where(lambda);
                    }
                }
            }
            return source;
        }

    }
}
