using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WI.ApiBoilerplate.Swagger
{
	public class InfoApiVersionDto
	{
		public string TitleTemplate { get; set; }
		public string VersionTemplate { get; set; }
		public string Description { get; set; }
		public string ContactName { get; set; }
		public string ContactEmail { get; set; }
		public string TermsOfService { get; set; }
		public string LicenseName { get; set; }
		public string LicenseUrl { get; set; }
		public string DescriptionPartWhenDepricated { get; set; }
		public string XmlCommentsFilePath { get; set; }
	}
}
