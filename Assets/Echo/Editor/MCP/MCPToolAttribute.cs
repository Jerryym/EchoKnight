using System;

namespace Echo.Editor.MCP
{
	/// <summary>
	/// MCP 工具标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class MCPToolAttribute : Attribute
	{
		public string Method { get; }

		public MCPToolAttribute(string method)
		{
			Method = method;
		}
	}
}
