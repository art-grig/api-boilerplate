using System;
using System.Collections.Generic;
using System.Text;

namespace WI.ApiBoilerplate.Domain.Infrastructure
{
	public interface IBaseEntity<TKey> : IIdentity<TKey> where TKey : struct
	{
		bool IsDeleted { get; set; }
	}

	public interface IBaseEntity : IBaseEntity<Guid> { }
}
