using System;
using System.Collections.Generic;
using System.Text;

namespace WI.Foundation.ViewModels
{
	public class ResponseVm<T> : BaseResponseVm
	{
		public ResponseVm() : base() { }
		public ResponseVm(ResponseTypes result, string userMessage) : base(result, userMessage) { }
		public ResponseVm(Exception exception, string userMessage = null) : base(exception, userMessage) { }

		public ResponseVm(T data)
		{
			Response = data;
			Result = data != null ? ResponseTypes.Success : ResponseTypes.NotFound;
		}

		public T Response { get; set; }
	}
}
