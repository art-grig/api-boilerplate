using System;
using System.Collections.Generic;
using System.Text;

namespace WI.ApiBoilerplate.Domain.Infrastructure
{
	public class Identity<TKey> : IIdentity<TKey> where TKey : struct
	{
		public TKey Id { get; set; }
	}
}
