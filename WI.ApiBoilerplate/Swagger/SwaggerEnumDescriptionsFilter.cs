using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WI.ApiBoilerplate.Swagger
{
	public class SwaggerEnumDescriptionsFilter : IDocumentFilter
	{
		public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
		{
			// add enum descriptions to result models
			foreach (var schemaDictionaryItem in swaggerDoc.Definitions)
			{
				var schema = schemaDictionaryItem.Value;
				foreach (var propertyDictionaryItem in schema.Properties)
				{
					var property = propertyDictionaryItem.Value;
					var propertyEnums = property.Enum;
					if (propertyEnums != null && propertyEnums.Any())
					{
						property.Description += DescribeEnum(propertyEnums);
					}
				}
			}

			// add enum descriptions to input parameters
			if (swaggerDoc.Paths.Any())
			{
				foreach (var pathItem in swaggerDoc.Paths.Values)
				{
					// head, patch, options, delete left out
					var possibleParameterisedOperations = new List<Operation>
					{
						pathItem.Get,
						pathItem.Post,
						pathItem.Put,
						pathItem.Delete,
					};
					possibleParameterisedOperations.ForEach(x => DescribeEnumParameters(x?.Parameters));
				}
			}

		}

		private static string DescribeEnum(IList<object> enums)
		{
			var description = string.Join(", ", enums.Select(enumOption => $"{(int)enumOption} = {Enum.GetName(enumOption.GetType(), enumOption)}"));
			return description;
		}

		private void DescribeEnumParameters(IList<IParameter> parameters)
		{
			if (parameters == null) return;

			foreach (var param in parameters)
			{
				var paramEnums = (param as NonBodyParameter)?.Enum;
				if (paramEnums != null && paramEnums.Any())
				{
					param.Description += DescribeEnum(paramEnums);
				}
			}
		}
	}
}
