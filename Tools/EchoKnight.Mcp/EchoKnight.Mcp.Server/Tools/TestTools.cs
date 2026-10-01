using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Echo.Mcp.Tools
{
	public enum TestMode
	{
		Fast,
		Normal,
		HighQuality
	}

	public class TestPosition
	{
		[Description("X 坐标")]
		public float X { get; set; }

		[Description("Y 坐标")]
		public float Y { get; set; }

		[Description("Z 坐标")]
		public float Z { get; set; }
	}

	public class TestTarget
	{
		[Description("目标名称")]
		public string Name { get; set; }

		[Description("目标位置")]
		public TestPosition Position { get; set; }
	}

	public class TestSetting
	{
		[Description("任务名称")]
		public string Name { get; set; }

		[Description("执行模式")]
		public TestMode Mode { get; set; }

		[Description("原点位置")]
		public TestPosition Origin { get; set; }

		[Description("目标列表")]
		public List<TestTarget> Targets { get; set; }
	}

	[McpServerToolType]
	public static class TestTools
	{
		/// <summary>
		/// 回显指定消息
		/// </summary>
		/// <param name="message">消息内容</param>
		/// <returns>原始消息</returns>
		[McpServerTool]
		[Description("返回传入的消息，用于验证 MCP Tool 调用链路")]
		public static string Echo([Description("需要回显的消息")] string message)
		{
			return message;
		}

		/// <summary>
		/// 测试复杂参数的 MCP Schema 映射
		/// </summary>
		/// <param name="setting">测试配置</param>
		/// <returns>测试结果</returns>
		[McpServerTool]
		[Description("测试复杂 C# 类型到 MCP Tool Schema 的映射")]
		public static string TestSchema([Description("测试配置")] TestSetting setting)
		{
			return $"Name: {setting.Name}, " + $"Mode: {setting.Mode}, " + $"TargetCount: {setting.Targets?.Count ?? 0}";
		}
	}
}
