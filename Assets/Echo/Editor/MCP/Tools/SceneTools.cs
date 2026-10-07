using Echo.Mcp.Protocol;
using System.Threading.Tasks;

namespace Echo.Editor.MCP
{
	/// <summary>
	/// 场景工具集
	/// </summary>
	public static class SceneTools
	{
		[MCPTool("scene.query")]
		public static Task<string> QueryScene(IPCRequest request)
		{
			string result = "{\r\n    \"name\":\"SampleScene\",\r\n    \"objects\":[\r\n        \"Camera\",\r\n        \"Directional Light\",\r\n        \"Cube\"\r\n    ]\r\n}";
			return Task.FromResult(result);
		}
	}
}
