using Echo.Editor.MCP;
using Echo.Mcp.Protocol;
using System.Threading.Tasks;
using UnityEditor;

namespace Echo.Editor.IPC
{
	/// <summary>
	/// IPC 服务启动入口
	/// </summary>
	[InitializeOnLoad]
	public static class IPCServerBootstrap
	{
		/// <summary>
		/// IPC服务器
		/// </summary>
		private static IPCServer s_server;

		static IPCServerBootstrap()
		{
			AssemblyReloadEvents.beforeAssemblyReload += Stop;
			EditorApplication.quitting += Stop;

			Start();
		}

		private static void Start()
		{
			if (s_server != null)
				return;

			RequestDispatcher dispatcher = new RequestDispatcher();
			//注册MCP工具
			RegisterMCPTool(dispatcher);

			s_server = new IPCServer(dispatcher);
			s_server.Start();
		}

		private static void Stop()
		{
			if (s_server == null)
				return;

			s_server.Dispose();
			s_server = null;
		}

		/// <summary>
		/// 注册MCP工具
		/// </summary>
		private static void RegisterMCPTool(RequestDispatcher dispatcher)
		{
			//场景工具集
			dispatcher.Register(typeof(SceneTools).Assembly);
		}
	}
}
