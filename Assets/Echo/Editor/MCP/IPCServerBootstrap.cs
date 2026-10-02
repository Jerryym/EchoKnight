using UnityEditor;
using UnityEngine;

namespace Echo.Editor.MCP
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

			s_server = new IPCServer();
			s_server.Start();
		}

		private static void Stop()
		{
			if (s_server == null)
				return;

			s_server.Dispose();
			s_server = null;
		}
	}
}
