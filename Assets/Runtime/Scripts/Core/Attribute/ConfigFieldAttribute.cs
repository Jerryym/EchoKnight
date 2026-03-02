using System;

namespace Echo
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ConfigFieldAttribute : Attribute
	{
		public string FieldName { get; }

		public ConfigFieldAttribute(string fieldName)
		{
			FieldName = fieldName;
		}
	}
}
