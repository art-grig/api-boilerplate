using System;
using System.Collections.Generic;
using System.Text;

namespace WI.Foundation.ViewModels
{
	public class ValidationIssueVm
	{
		public string Description { get; private set; }
		public string SystemMessage { get; private set; }
		public bool CanBeSkipped { get; private set; }
		public object SystemInfo { get; private set; }

		public ValidationIssueVm(string description, string systemMessage, bool canBeSkipped, object systemInfo = null)
		{
			Description = description;
			SystemMessage = systemMessage;
			CanBeSkipped = canBeSkipped;
			SystemInfo = systemInfo;
		}
	}
}
