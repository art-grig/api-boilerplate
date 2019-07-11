using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WI.ApiBoilerplate.Domain.Infrastructure;
using WI.ApiBoilerplate.ORM.Expressions;
using WI.ApiBoilerplate.ORM.Extensions;
using WI.ApiBoilerplate.ORM.Infrastructure;
using WI.Foundation.Exceptions;

namespace WI.ApiBoilerplate.ORM.Repositories
{
	public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> 
		where T : class, IIdentity<TKey>, IBaseEntity<TKey>
		where TKey : struct
	{
		protected readonly ApplicationDbContext _bookedByDbContext;
		protected readonly IMapper _mapper;

		public GenericRepository(ApplicationDbContext context, IMapper mapper)
		{
			_mapper = mapper;
			_bookedByDbContext = context;
		}

		protected DbSet<T> Entities => _bookedByDbContext.Set<T>();
		protected IQueryable<T> NotDeletedEntities => Entities.NotDeleted();

		public async Task<T> GetById(TKey id, bool includeDeleted = false, CancellationToken ct = default(CancellationToken))
		{
			var source = includeDeleted ? Entities : NotDeletedEntities;

			var entity = await source.ById(id).SingleOrDefaultAsync(ct);

			return entity as T;
		}

		public async Task<List<T>> GetByIds(List<TKey> ids, bool includeDeleted = false, CancellationToken ct = default(CancellationToken))
		{
			var source = includeDeleted ? Entities : NotDeletedEntities;
			var entities = await source.Where(GenericExpressions<T, TKey>.IsOneOf(ids)).ToListAsync(ct);
			return entities;
		}

		public async Task<TProjection> GetProjectionById<TProjection>(TKey id, bool includeDeleted = false, CancellationToken ct = default(CancellationToken))
		{
			var source = includeDeleted ? Entities : NotDeletedEntities;

			var entity = await source.ById(id).ProjectTo<TProjection>(_mapper.ConfigurationProvider).SingleOrDefaultAsync(ct);

			return entity;
		}

		public async Task<List<TProjection>> GetProjectionByIds<TProjection>(List<TKey> ids, bool includeDeleted = false, CancellationToken ct = default(CancellationToken))
		{
			var source = includeDeleted ? Entities : NotDeletedEntities;
			var projections = await source.Where(GenericExpressions<T, TKey>.IsOneOf(ids)).ProjectTo<TProjection>(_mapper.ConfigurationProvider).ToListAsync(ct);
			return projections;
		}

		public async Task<List<TResult>> ProjectToAsync<TResult>(bool findDeleted = false)
		{
			var query = findDeleted ? Entities.AsQueryable() : NotDeletedEntities;

			return await query.ProjectTo<TResult>(_mapper.ConfigurationProvider).ToListAsync();
		}

		public async Task<List<TResult>> ProjectToAsync<TResult>(Expression<Func<T, bool>> predicate, bool findDeleted = false)
		{
			return await ProjectToAsync<TResult>(predicate, findDeleted, new Expression<Func<T, BaseEntity>>[0]);
		}

		public async Task<List<TResult>> ProjectToAsync<TResult>(bool findDeleted = false, params Expression<Func<T, BaseEntity>>[] selectors)
		{
			return await ProjectToAsync<TResult>(i => true, findDeleted, selectors);
		}

		public async Task<List<TResult>> ProjectToAsync<TResult>(Expression<Func<T, bool>> predicate, bool findDeleted = false, params Expression<Func<T, BaseEntity>>[] selectors)
		{
			var query = findDeleted ? Entities.AsQueryable() : NotDeletedEntities;

			foreach (var selector in selectors)
			{
				query = query.Include(selector);
			}

			return await query.Where(predicate).ProjectTo<TResult>(_mapper.ConfigurationProvider).ToListAsync();
		}

		public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null, bool includeDeleted = false, CancellationToken ct = default(CancellationToken))
		{
			var source = includeDeleted ? Entities : NotDeletedEntities;

			var result = predicate == null ? await source.FirstOrDefaultAsync(ct) : await source.FirstOrDefaultAsync(predicate, ct);

			return result;
		}

		public async Task<T> SingleAsync(Expression<Func<T, bool>> predicate = null, bool includeDeleted = false, CancellationToken ct = default(CancellationToken))
		{
			var source = includeDeleted ? Entities : NotDeletedEntities;

			var result = predicate == null ? await source.SingleAsync(ct) : await source.SingleAsync(predicate, ct);

			return result;
		}

		public async Task Delete(TKey id, bool hard = false, CancellationToken ct = default(CancellationToken))
		{
			var entity = await GetById(id, hard, ct); // Include deleted items to be able to hard delete them
			if (entity == null)
				throw new WebImpactNotFoundException($"Not found (id={id}).");

			if (hard)
			{
				Entities.Remove(entity);
			}
			else
			{
				entity.IsDeleted = true;
			}
		}

		public async Task Delete(IEnumerable<TKey> ids, bool hard = false, CancellationToken ct = default(CancellationToken))
		{
			var entities = await (hard
				? Entities
				: NotDeletedEntities).Where(GenericExpressions<T, TKey>.IsOneOf(ids)).ToListAsync(ct);

			if (hard)
			{
				Entities.RemoveRange(entities);
			}
			else
			{
				foreach (var entity in entities)
				{
					entity.IsDeleted = true;
				}
			}
		}

		public void Restore(T entity)
		{
			if (entity == null)
				throw new ArgumentNullException("Entity is null");
			entity.IsDeleted = false;
		}

		public async Task RestoreAsync(TKey id, CancellationToken ct = default(CancellationToken))
		{
			var entity = await GetById(id, true, ct);
			if (entity == null)
				throw new WebImpactNotFoundException($"Not found (id={id}).");

			entity.IsDeleted = false;
		}

		public async Task RestoreAsync(IEnumerable<TKey> ids, CancellationToken ct = default(CancellationToken))
		{
			var entities = await Entities.Where(GenericExpressions<T, TKey>.IsOneOf(ids)).ToListAsync(ct);
			foreach (var entity in entities)
			{
				entity.IsDeleted = false;
			}
		}

		public async Task<IList<TSelection>> Select<TSelection>(Expression<Func<T, TSelection>> selector, bool findDeleted = false, CancellationToken ct = default(CancellationToken))
		{
			var query = findDeleted ? Entities : NotDeletedEntities;
			return await query.Select(selector).ToListAsync(ct);
		}

		public async Task<IList<TSelection>> Select<TSelection>(Expression<Func<T, bool>> predicate, Expression<Func<T, TSelection>> selector, bool findDeleted = false, CancellationToken ct = default(CancellationToken))
		{
			var query = findDeleted ? Entities : NotDeletedEntities;
			return await query.Where(predicate).Select(selector).ToListAsync(ct);
		}

		public async Task InsertAsync(T entity, CancellationToken ct = default(CancellationToken))
		{
			await Entities.AddAsync(entity, ct);
		}

		public async Task InsertReusingDeletedAsync(T entity, CancellationToken ct = default(CancellationToken))
		{
			var deleted = await Entities.Where(x => x.IsDeleted).FirstOrDefaultAsync(ct);
			if (deleted != null)
			{
				entity.Id = deleted.Id;
				entity.IsDeleted = false;
				_bookedByDbContext.Entry(deleted).State = EntityState.Detached;
				Entities.Update(entity);
			}
			else
			{
				await Entities.AddAsync(entity, ct);
			}
		}

		public async Task InsertAsync(IEnumerable<T> entities, CancellationToken ct = default(CancellationToken))
		{
			await Entities.AddRangeAsync(entities, ct);
		}

		public async Task UpdateAsync(T entity)
		{
			await Task.FromResult(Entities.Update(entity));
		}

		public async Task<bool> Any(TKey id, bool includeDeleted = false, CancellationToken ct = default(CancellationToken))
		{
			var source = includeDeleted ? Entities : NotDeletedEntities;
			var any = await source.ById(id).AnyAsync(ct);
			return any;
		}

		public async Task<EntityContainer<T, TKey>> InsertMapped<TModel>(TModel sourceModel, CancellationToken ct = default(CancellationToken))
		{
			var entity = _mapper.Map<T>(sourceModel);
			await InsertAsync(entity, ct);

			return EntityContainer<T, TKey>.FromEntity(entity);
		}

		public async Task<List<EntityContainer<T, TKey>>> InsertMapped<TModel>(IEnumerable<TModel> sourceModel, CancellationToken ct = default(CancellationToken))
		{
			var entities = _mapper.Map<IEnumerable<T>>(sourceModel);
			await InsertAsync(entities, ct);
			return entities.Select(x => EntityContainer<T, TKey>.FromEntity(x)).ToList();
		}

		public async Task<EntityContainer<T, TKey>> InsertMappedReusingDeleted<TModel>(TModel sourceModel, CancellationToken ct = default(CancellationToken))
		{
			var entity = _mapper.Map<T>(sourceModel);
			await InsertReusingDeletedAsync(entity, ct);

			return EntityContainer<T, TKey>.FromEntity(entity);
		}

		public async Task<EntityContainer<T, TKey>> UpdateAsync<TModel>(TKey id, TModel sourceModel, CancellationToken ct = default(CancellationToken))
		{
			var entity = await Entities.FindAsync(new object[] { id }, ct);
			_mapper.Map(sourceModel, entity);
			await UpdateAsync(entity);

			return EntityContainer<T, TKey>.FromEntity(entity);
		}
	}
}
