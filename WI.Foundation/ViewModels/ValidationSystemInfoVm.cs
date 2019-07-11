using System;
using System.Collections.Generic;
using System.Text;

namespace WI.Foundation.ViewModels
{
	public class ValidationSystemInfoVm
	{
		public ValidationSystemInfoVm(string message, object info)
		{
			UserMessage = message;
			SystemInfo = info;
		}

		public string UserMessage { get; set; }
		public object SystemInfo { get; set; }

		public override string ToString()
		{
			return UserMessage;
		}
	}
}
