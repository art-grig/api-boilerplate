using System;
using System.Collections.Generic;

namespace WI.Foundation.Helpers.Rest.Models.Interfaces
{
	public interface IHttpRequest
    {
		/// <summary>
		/// Prevents from throwing WebImpactExternalRequestException if response status is not successful
		/// </summary>
		IHttpRequest ForAnyResponseStatus();
		/// <summary>
		/// Sets the relative endpoint address. From and To methods are absolutely identical.
		/// </summary>
		IAnyRequest From(string relativeAddress);
		/// <summary>
		/// Sets the relative endpoint address. From and To methods are absolutely identical.
		/// </summary>
		IAnyRequest To(string relativeAddress);

		IParallelRequest InParallel(Func<IHttpRequest, IEnumerable<IAnyRequest>> multiplicator);
		IParallelRequest InParallel(Func<IHttpRequest, IEnumerable<IHttpPayloadRequest>> multiplicator);
	}
}
