using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WI.ApiBoilerplate.Domain.Infrastructure;

namespace WI.ApiBoilerplate.ORM.Extensions
{
	public static class QueryableExtensions
	{
		private const string IsDeleted = nameof(IBaseEntity.IsDeleted);
		private const string Id = nameof(IBaseEntity.Id);

		public static IQueryable<T> NotDeleted<T>(this IQueryable<T> q)
		{
			if (typeof(T).GetProperty(IsDeleted) == null)
				throw new InvalidOperationException($"{typeof(T)} does not contain property {IsDeleted}.");

			var parameter = Expression.Parameter(typeof(T));
			var notDeleted = Expression.Lambda<Func<T, bool>>(
				Expression.Not(Expression.Property(parameter, IsDeleted)),
				parameter);

			return q.Where(notDeleted);
		}

		public static IQueryable<IIdentity<TKey>> ById<TKey>(this IQueryable<IIdentity<TKey>> q, TKey id) where TKey : struct
		{
			var parameter = Expression.Parameter(typeof(TKey));
			var byId = Expression.Lambda<Func<IIdentity<TKey>, bool>>(
				Expression.Equal(
					Expression.Property(parameter, Id),
					Expression.Constant(id)),
				parameter);

			return q.Where(byId);
		}
	}
}
