using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WI.ApiBoilerplate.Domain.Infrastructure;

namespace WI.ApiBoilerplate.ORM.Expressions
{
	public class GenericExpressions<T, TKey> where T : IBaseEntity<TKey>
		where TKey : struct
	{
		public static Expression<Func<T, bool>> IsNotDeleted =>
			e => !e.IsDeleted;

		public static Expression<Func<T, bool>> IsOneOf(IEnumerable<TKey> ids) =>
			e => ids.Contains(e.Id);

		public static Expression<Func<T, bool>> ById(TKey id) =>
			e => e.Id.Equals(id);

		public static Expression<Func<T, TKey>> IdOnly =>
			e => e.Id;
	}
}
