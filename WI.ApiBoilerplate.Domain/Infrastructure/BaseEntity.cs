using System;
using System.Collections.Generic;
using System.Text;

namespace WI.ApiBoilerplate.Domain.Infrastructure
{
	public class BaseEntity<TKey> : Identity<TKey>, IBaseEntity<TKey>
		where TKey : struct
	{
		public bool IsDeleted { get; set; }
	}

	public class BaseEntity : BaseEntity<Guid>, IBaseEntity { }
}
