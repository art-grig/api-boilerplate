using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WI.ApiBoilerplate.Helpers;
using WI.Foundation.Helpers;
using WI.Foundation.Exceptions;
using WI.Foundation.ViewModels;

namespace WI.ApiBoilerplate.Controllers
{
	[ApiController]
	public abstract class ApiController : Controller
	{
		protected void ValidateIds(params int[] ids)
		{
			foreach (var id in ids)
			{
				if (id <= 0)
					throw new WebImpactValidationException("Cannot process empty request");
			}
		}

		protected void ValidatePayload(object payload)
		{
			if (payload == null)
				throw new WebImpactValidationException("Cannot process empty request");
		}

		protected IActionResult Resp(MediaFileVm mediaFile)
		{
			return File(mediaFile.Content, mediaFile.ContentType);
		}

		protected ActionResult<BaseResponseVm> ResponseTyped(BaseResponseVm response)
		{
			return this.RespInnerTyped(response);
		}

		protected ActionResult<T> RespInnerTyped<T>(T responseVM) where T : BaseResponseVm
		{
			var logger = WebImpactLogger.CreateLogger();
			if (logger.IsEnabled(LogLevel.Debug))
			{
				string message = JsonConvert.SerializeObject((object)responseVM);
				logger.LogDebug(message);
			}
			return this.StatusCode(StatusCodeHelper.GetIntStatusCodeForResponse(responseVM), (object)responseVM);
		}
	}
}
