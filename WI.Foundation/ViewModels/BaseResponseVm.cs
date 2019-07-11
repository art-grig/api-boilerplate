using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WI.Foundation.Exceptions;

namespace WI.Foundation.ViewModels
{
	public class BaseResponseVm
	{
		public BaseResponseVm()
		{
			GeneratedAt = DateTime.UtcNow;
			Result = ResponseTypes.Success;
		}

		public BaseResponseVm(ResponseTypes result, string userMessage)
			: this()
		{
			Result = result;
			UserMessage = userMessage;
		}

		public BaseResponseVm(Exception exception, string userMessage = null)
			: this()
		{
			if (exception is WebImpactBaseException)
			{
				var ex = (exception as WebImpactBaseException);
				Result = ex.ResponseType;
				UserMessage = userMessage ?? ex.Message;
				SystemMessage = ex.SystemMessage;
				DebugInfo = ex.WebImpactDebugInfo;
			}
			else if (exception is NotImplementedException)
			{
				Result = ResponseTypes.NotImplemented;
				UserMessage = "This API endpoint is not completely implemented so far.";
			}
			else
			{
				Result = ResponseTypes.Exception;
				UnwrapException(exception);
			}
		}

		public enum ResponseTypes
		{
			NotImplemented = -1,
			Success = 1,
			NotAuthenticated = 2,
			NoPermission = 3,
			FailedValidation = 4,
			Exception = 5,
			NotFound = 6,
			BadData = 7
		}

		public DateTime? GeneratedAt { get; set; }

		public ResponseTypes Result { get; set; }
		public string UserMessage { get; set; }
		public string SystemMessage { get; set; }

		public AccessRefreshTokensVm AuthTokens { get; set; }

		// This will probably be used later.
#if !DEBUG
		[JsonIgnore]
#endif
		public object DebugInfo { get; set; }

		public string SystemErrorMessage { get; set; }
		public string SystemErrorType { get; set; }
		public string SystemErrorStack { get; set; }

		private void UnwrapException(Exception e)
		{
			var flatten = FlattenException(e);
			foreach (var ie in flatten)
			{
				SystemErrorType += ie.GetType().FullName + Environment.NewLine;
				SystemErrorMessage += ie.Message + Environment.NewLine;
			}
			SystemErrorType = SystemErrorType.TrimEnd();
			SystemErrorMessage = SystemErrorMessage.TrimEnd();
			SystemErrorStack = e.StackTrace;
		}

		private IEnumerable<Exception> FlattenException(Exception e)
		{
			var ae = e as AggregateException;
			if (ae != null && ae.InnerExceptions.Count > 0)
			{
				yield return e;
				foreach (var ie in ae.InnerExceptions)
				{
					var result = FlattenException(ie);
					foreach (var r in result)
						yield return r;
				}
			}
			else
			if (e.InnerException != null)
			{
				yield return e;
				var result = FlattenException(e.InnerException);
				foreach (var r in result)
					yield return r;
			}
			else
				yield return e;
		}
	}
}
