using Echo.Mcp.Bridge;

namespace EchoKnight.Mcp.Server
{
	public static class BridgeManager
	{
		private static EchoKnightBridge? s_Instance;

		public static async Task<EchoKnightBridge> GetBridgeAsync()
		{
			if (s_Instance != null)
				return s_Instance;

			s_Instance = new EchoKnightBridge();
			await s_Instance.ConnectAsync();

			return s_Instance;
		}

		public static void Dispose()
		{
			s_Instance?.Dispose();
			s_Instance = null;
		}
	}
}
