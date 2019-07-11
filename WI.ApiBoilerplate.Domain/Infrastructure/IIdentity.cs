using System;
using System.Collections.Generic;
using System.Text;

namespace WI.ApiBoilerplate.Domain.Infrastructure
{
	public interface IIdentity<TKey> where TKey : struct
	{
		TKey Id { get; set; }
	}
}
