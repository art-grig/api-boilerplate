using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WI.Foundation.Exceptions;
using WI.Foundation.ViewModels;

namespace WI.Foundation.Validation
{
	public class ValidationResult
	{

		public List<ValidationIssueVm> Issues { get => _issues ?? (_issues = new List<ValidationIssueVm>()); set => _issues = value; }
		public bool IsValid => !Errors.Any();

		#region Protected properties

		protected IEnumerable<ValidationIssueVm> Errors => Issues?.Where(x => !x.CanBeSkipped) ?? Enumerable.Empty<ValidationIssueVm>();
		protected IEnumerable<ValidationIssueVm> Warnings => Issues?.Where(x => x.CanBeSkipped) ?? Enumerable.Empty<ValidationIssueVm>();
		protected List<ValidationIssueVm> _issues;

		#endregion

		#region Public properties

		public string ErrorsToString => Errors.Any()
			? $"Errors:{Environment.NewLine}{string.Join(Environment.NewLine, Errors.Select(x => x.Description))}"
			: string.Empty;

		public string WarningsToStrings => Warnings.Any()
			? $"Warnings:{Environment.NewLine}{string.Join(Environment.NewLine, Warnings.Select(x => x.Description))}"
			: string.Empty;

		public string AllIssuesToString => (ErrorsToString + Environment.NewLine + WarningsToStrings).Trim();



		public string SystemErrorsToString => Errors.Any()
			? $"Errors:{Environment.NewLine}{string.Join(Environment.NewLine, Errors.Select(x => x.SystemMessage))}"
			: string.Empty;

		public string SystemWarningsToStrings => Warnings.Any()
			? $"Warnings:{Environment.NewLine}{string.Join(Environment.NewLine, Warnings.Select(x => x.SystemMessage))}"
			: string.Empty;

		public string AllSystemMessagesToString => (SystemErrorsToString + Environment.NewLine + SystemWarningsToStrings).Trim();



		public List<ValidationSystemInfoVm> ErrorsDebugInfo => Errors.Any()
			? Errors.Select(x => new ValidationSystemInfoVm(x.Description, x.SystemInfo)).ToList()
			: null;

		public List<ValidationSystemInfoVm> WarningsDebugInfo => Warnings.Any()
			? Warnings.Select(x => new ValidationSystemInfoVm(x.Description, x.SystemInfo)).ToList()
			: null;

		public List<ValidationSystemInfoVm> AllIssuesDebugInfo =>
			Issues.Select(x => new ValidationSystemInfoVm(x.Description, x.SystemInfo)).ToList();

		#endregion

		#region Constructors

		public ValidationResult(string message, bool canBeSkipped, object systemInfo = null) : this(message, message, canBeSkipped, systemInfo)
		{
		}

		public ValidationResult(string message, string systemMessage, bool canBeSkipped, object systemInfo = null) : base()
		{
			AddIssue(message, systemMessage, canBeSkipped, systemInfo);
		}

		public ValidationResult()
		{
			Issues = new List<ValidationIssueVm>();
		}

		#endregion

		#region Public methods

		public void AddResult(ValidationResult result)
		{
			if (result.Issues == null)
				return;

			if (Issues == null)
				Issues = result.Issues;
			else
				Issues.AddRange(result.Issues);
		}

		public void AddError(string errorMessage, object systemInfo = null)
		{
			AddError(errorMessage, errorMessage, systemInfo);
		}

		public void AddError(string errorMessage, string systemMessage, object systemInfo = null)
		{
			AddIssue(errorMessage, systemMessage, false, systemInfo);
		}

		public void AddWarning(string warningMessage, object systemInfo = null)
		{
			AddWarning(warningMessage, warningMessage, systemInfo);
		}

		public void AddWarning(string warningMessage, string systemMessage, object systemInfo = null)
		{
			AddIssue(warningMessage, systemMessage, true, systemInfo);
		}

		public void ThrowIfNotValid()
		{
			if (!this.IsValid)
				throw new WebImpactValidationException(this);
		}

		#endregion

		protected void AddIssue(string issueMessage, string systemMessage, bool canBeSkipped, object systemInfo = null)
		{
			if (Issues == null)
				Issues = new List<ValidationIssueVm>();

			Issues.Add(new ValidationIssueVm(issueMessage, systemMessage, canBeSkipped, systemInfo));
		}
	}
}
