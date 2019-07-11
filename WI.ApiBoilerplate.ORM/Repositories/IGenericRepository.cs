using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WI.ApiBoilerplate.Domain.Infrastructure;
using WI.ApiBoilerplate.ORM.Infrastructure;

namespace WI.ApiBoilerplate.ORM.Repositories
{
	public interface IGenericRepository<T, TKey> 
		where T : class, IIdentity<TKey>, IBaseEntity<TKey>
		where TKey : struct
	{
		Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null, bool includeDeleted = false, CancellationToken ct = default(CancellationToken));


		Task<T> SingleAsync(Expression<Func<T, bool>> predicate = null, bool includeDeleted = false, CancellationToken ct = default(CancellationToken));


		Task Delete(TKey id, bool hard = false, CancellationToken ct = default(CancellationToken));

		Task Delete(IEnumerable<TKey> ids, bool hard = false, CancellationToken ct = default(CancellationToken));

		void Restore(T entity);
		Task RestoreAsync(TKey id, CancellationToken ct = default(CancellationToken));
		Task RestoreAsync(IEnumerable<TKey> ids, CancellationToken ct = default(CancellationToken));


		Task InsertAsync(T entity, CancellationToken ct = default(CancellationToken));

		Task InsertAsync(IEnumerable<T> entities, CancellationToken ct = default(CancellationToken));

		Task InsertReusingDeletedAsync(T entity, CancellationToken ct = default(CancellationToken));

		Task<bool> Any(TKey id, bool includeDeleted = false, CancellationToken ct = default(CancellationToken));

		Task<T> GetById(TKey id, bool includeDeleted = false, CancellationToken ct = default(CancellationToken));
		Task<List<T>> GetByIds(List<TKey> ids, bool includeDeleted = false, CancellationToken ct = default(CancellationToken));

		Task<TProjection> GetProjectionById<TProjection>(TKey id, bool includeDeleted = false, CancellationToken ct = default(CancellationToken));
		Task<List<TProjection>> GetProjectionByIds<TProjection>(List<TKey> ids, bool includeDeleted = false, CancellationToken ct = default(CancellationToken));

		Task<List<TResult>> ProjectToAsync<TResult>(Expression<Func<T, bool>> predicate, bool findDeleted = false);
		Task<List<TResult>> ProjectToAsync<TResult>(bool findDeleted = false, params Expression<Func<T, BaseEntity>>[] selectors);
		Task<List<TResult>> ProjectToAsync<TResult>(Expression<Func<T, bool>> predicate, bool findDeleted = false, params Expression<Func<T, BaseEntity>>[] selectors);

		Task UpdateAsync(T entity);

		Task<IList<TSelection>> Select<TSelection>(Expression<Func<T, TSelection>> selector, bool findDeleted = false, CancellationToken ct = default(CancellationToken));
		Task<IList<TSelection>> Select<TSelection>(Expression<Func<T, bool>> predicate,
			Expression<Func<T, TSelection>> selector, bool findDeleted = false, CancellationToken ct = default(CancellationToken));

		/// <summary>
		/// Maps <paramref name="sourceModel"/> to <see cref="T"/> entity and inserts the result.
		/// </summary>
		/// <param name="sourceModel">Sorce model with data to map and insert.</param>
		Task<EntityContainer<T, TKey>> InsertMapped<TModel>(TModel sourceModel, CancellationToken ct = default(CancellationToken));
		Task<List<EntityContainer<T, TKey>>> InsertMapped<TModel>(IEnumerable<TModel> sourceModel, CancellationToken ct = default(CancellationToken));

		Task<EntityContainer<T, TKey>> InsertMappedReusingDeleted<TModel>(TModel sourceModel, CancellationToken ct = default(CancellationToken));

		/// <summary>
		/// Updates the entity (specified by id) by mapping <paramref name="sourceModel"/> to it.
		/// </summary>
		/// <param name="id">Id of the entity to update.</param>
		/// <param name="sourceModel">Sorce model with data to map and insert.</param>
		Task<EntityContainer<T, TKey>> UpdateAsync<TModel>(TKey id, TModel sourceModel, CancellationToken ct = default(CancellationToken));
	}
}
