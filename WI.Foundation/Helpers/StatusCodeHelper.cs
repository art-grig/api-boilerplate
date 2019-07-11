using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using WI.Foundation.ViewModels;

namespace WI.Foundation.Helpers
{
	public class StatusCodeHelper
	{
		private static readonly Dictionary<BaseResponseVm.ResponseTypes, HttpStatusCode> _statusMap = new Dictionary<BaseResponseVm.ResponseTypes, HttpStatusCode>()
		{
			{ BaseResponseVm.ResponseTypes.NotImplemented, HttpStatusCode.NotImplemented },
			{ BaseResponseVm.ResponseTypes.Success, HttpStatusCode.OK },
			{ BaseResponseVm.ResponseTypes.NotAuthenticated, HttpStatusCode.Unauthorized },
			{ BaseResponseVm.ResponseTypes.NoPermission, HttpStatusCode.Forbidden },
			{ BaseResponseVm.ResponseTypes.FailedValidation, HttpStatusCode.BadRequest },
			{ BaseResponseVm.ResponseTypes.Exception, HttpStatusCode.InternalServerError },
			{ BaseResponseVm.ResponseTypes.NotFound, HttpStatusCode.NotFound },
			{ BaseResponseVm.ResponseTypes.BadData, HttpStatusCode.InternalServerError }
		};

		public static HttpStatusCode GetStatusCodeForResponse(BaseResponseVm response)
		{
			return _statusMap[response.Result];
		}

		public static int GetIntStatusCodeForResponse(BaseResponseVm response)
		{
			return (int)_statusMap[response.Result];
		}
	}
}
