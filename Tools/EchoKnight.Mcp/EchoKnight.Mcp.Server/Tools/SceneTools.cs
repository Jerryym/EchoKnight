using Echo.Mcp.Bridge;
using Echo.Mcp.Protocol;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace EchoKnight.Mcp.Server.Tools
{
	[McpServerToolType]
	public static class SceneTools
	{
		/// <summary>
		/// 查询当前 Unity 场景
		/// </summary>
		[McpServerTool(Name = "query_scene", ReadOnly = true, Destructive = false, Idempotent = true, OpenWorld = false)]
		[Description("查询当前 Unity Editor 场景信息")]
		public static async Task<IPCResponse> QueryScene()
		{
			EchoKnightBridge bridge = await BridgeManager.GetBridgeAsync();

			IPCRequest request = new IPCRequest
			{
				Id = Guid.NewGuid().ToString(),
				Method = "scene.query",
				Params = "{}"
			};

			IPCResponse? response = await bridge.RequestAsync(request);
			if (response == null)
				throw new Exception("Unity Editor disconnected");

			if (!response.Success)
				throw new Exception(response.Error.Message);

			return response;
		}
	}
}
