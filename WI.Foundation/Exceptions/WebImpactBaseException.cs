using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using WI.Foundation.Validation;
using WI.Foundation.ViewModels;

namespace WI.Foundation.Exceptions
{
	public abstract class WebImpactBaseException : Exception
	{
		public WebImpactBaseException(string userMessage) : base(userMessage) { }

		public WebImpactBaseException(string userMessage, Exception innerException) : base(userMessage, innerException) { }

		public WebImpactBaseException(string userMessage, object debugInfo) : base(userMessage)
		{
			WebImpactDebugInfo = debugInfo;
		}

		public WebImpactBaseException(string userMessage, string systemMessage) : base(userMessage)
		{
			SystemMessage = systemMessage;
		}

		public WebImpactBaseException(string userMessage, string systemMessage, object debugInfo) : this(userMessage, systemMessage)
		{
			WebImpactDebugInfo = debugInfo;
		}

		public abstract BaseResponseVm.ResponseTypes ResponseType { get; }
		public object WebImpactDebugInfo { get; protected set; }
		public string SystemMessage { get; protected set; }
	}

	public class WebImpactNotAuthenticatedException : WebImpactBaseException
	{
		public WebImpactNotAuthenticatedException(string userMessage) : base(userMessage) { }
		public override BaseResponseVm.ResponseTypes ResponseType => BaseResponseVm.ResponseTypes.NotAuthenticated;
	}

	public class WebImpactAccessDeniedException : WebImpactBaseException
	{
		public Dictionary<string, string> RequiredTasks { get; set; }

		public WebImpactAccessDeniedException(string userMessage) : base(userMessage) { }
		public override BaseResponseVm.ResponseTypes ResponseType => BaseResponseVm.ResponseTypes.NoPermission;
	}

	public class WebImpactNoPermissionException : WebImpactBaseException
	{
		public WebImpactNoPermissionException(string userMessage) : base(userMessage) { }
		public override BaseResponseVm.ResponseTypes ResponseType => BaseResponseVm.ResponseTypes.NoPermission;
	}

	public class WebImpactValidationException : WebImpactBaseException
	{
		public WebImpactValidationException(string userMessage) : base(userMessage) { }
		public WebImpactValidationException(string userMessage, object debugInfo) : base(userMessage, debugInfo) { }
		public WebImpactValidationException(string userMessage, string systemMessage, object debugInfo) : base(userMessage, systemMessage, debugInfo) { }
		public WebImpactValidationException(ValidationResult validationResult)
			: base(validationResult.AllIssuesToString, validationResult.AllSystemMessagesToString, validationResult.AllIssuesDebugInfo) { }

		public override BaseResponseVm.ResponseTypes ResponseType => BaseResponseVm.ResponseTypes.FailedValidation;
	}

	public class WebImpactNotFoundException : WebImpactBaseException
	{
		public WebImpactNotFoundException(string userMessage) : base(userMessage) { }
		public override BaseResponseVm.ResponseTypes ResponseType => BaseResponseVm.ResponseTypes.NotFound;
	}

	public class WebImpactExternalResourceException : WebImpactBaseException
	{
		public WebImpactExternalResourceException(string userMessage) : base(userMessage) { }
		public WebImpactExternalResourceException(string userMessage, string systemMessage) : base(userMessage, systemMessage) { }
		public WebImpactExternalResourceException(string userMessage, string systemMessage, object responseDebugInfo) : base(userMessage, systemMessage, responseDebugInfo) { }
		public WebImpactExternalResourceException(string userMessage, string systemMessage, object responseDebugInfo, HttpRequestMessage requestData) : base(userMessage, responseDebugInfo)
		{
			SystemMessage = systemMessage;
			RequestData = requestData;
		}
		public override BaseResponseVm.ResponseTypes ResponseType => BaseResponseVm.ResponseTypes.Exception;

		public HttpRequestMessage RequestData { get; private set; }
	}

	public class WebImpactDisabledFeatureException : WebImpactBaseException
	{
		public WebImpactDisabledFeatureException(string featureName) : base($"{featureName} feature is switched off.") { }
		public override BaseResponseVm.ResponseTypes ResponseType => BaseResponseVm.ResponseTypes.Exception;
	}

	public class WebImpactInvalidOperationException : WebImpactBaseException
	{
		public WebImpactInvalidOperationException(string userMessage) : base(userMessage) { }
		public override BaseResponseVm.ResponseTypes ResponseType => BaseResponseVm.ResponseTypes.Exception;
	}

	public class WebImpactBadDataException : WebImpactBaseException
	{
		public WebImpactBadDataException(string operationName, Exception sourceException)
			: base("Bad data has been detected in the database", sourceException)
		{
			OperationName = operationName;
		}

		public string OperationName { get; protected set; }
		public override BaseResponseVm.ResponseTypes ResponseType => BaseResponseVm.ResponseTypes.BadData;
	}
}
