using System;
using System.Collections.Generic;
using System.Text;
using WI.ApiBoilerplate.Domain.Infrastructure;

namespace WI.ApiBoilerplate.ORM.Infrastructure
{
	/// <summary>
	/// Represents a container for an entity to hide an entity and expose infrastructure properties.
	/// </summary>
	/// <typeparam name="TSource">Type of the entity.</typeparam>
	public class EntityContainer<TSource, TKey> 
		where TSource : class, IIdentity<TKey>, IBaseEntity<TKey>
		where TKey : struct
	{
		/// <summary>
		/// Initializes a new instanse of <see cref="EntityContainer{TSource}"/>.
		/// </summary>
		/// <param name="source"></param>
		private EntityContainer(TSource source)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		/// <summary> Creates new EntityContainer insance from given Entity </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static EntityContainer<T, TKey> FromEntity<T>(T entity) where T : class, IIdentity<TKey>, IBaseEntity<TKey>
		{
			var newContainer = new EntityContainer<T, TKey>(entity);

			return newContainer;
		}

		/// <summary>
		/// The identifier of the entity.
		/// </summary>
		public virtual TKey Id => Source.Id;

		/// <summary>
		/// Gets the flag whether entity was marked as deleted or not.
		/// </summary>
		public virtual bool IsDeleted => Source.IsDeleted;

		/// <summary>
		/// Gets the entity instance.
		/// </summary>
		protected virtual TSource Source { get; }

		/// <summary>
		/// Provides the way to map internal entity of this container to a dto.
		/// </summary>
		/// <typeparam name="TDestination">The type of the dto.</typeparam>
		/// <param name="mappingAction">The mapping delegate.</param>
		/// <returns>A mapped instance of the entity.</returns>
		public virtual TDestination MapTo<TDestination>(Func<TSource, TDestination> mappingAction)
		{
			return mappingAction(Source);
		}
	}
}
