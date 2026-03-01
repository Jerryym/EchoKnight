using System;

namespace Echo
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ConfigInfoAttribute : Attribute
	{
		public string ConfigName;

		public ConfigInfoAttribute(string configName)
		{
			ConfigName = configName;
		}
	}
}
