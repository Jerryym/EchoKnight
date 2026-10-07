using Echo.Editor.MCP;
using Echo.Mcp.Protocol;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Echo.Editor.IPC
{
	/// <summary>
	/// 请求分发器
	/// </summary>
	public class RequestDispatcher
	{
		/// <summary>
		/// 函数表
		/// </summary>
		private readonly Dictionary<string, Func<IPCRequest, Task<string>>> m_funcHandlers = new Dictionary<string, Func<IPCRequest, Task<string>>>();

		/// <summary>
		/// 注册函数
		/// </summary>
		public void Register(string funcName, Func<IPCRequest, Task<string>> handler)
		{
			if (string.IsNullOrEmpty(funcName))
				throw new ArgumentException(nameof(funcName));

			if (handler == null)
				throw new ArgumentNullException(nameof(handler));

			m_funcHandlers.Add(funcName, handler);
		}

		/// <summary>
		/// 注册函数
		/// </summary>
		public void Register(Assembly assembly)
		{
			foreach (Type type in assembly.GetTypes())
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo method in methods)
				{
					MCPToolAttribute mcpToolAttri = method.GetCustomAttribute<MCPToolAttribute>();
					if (mcpToolAttri == null)
						continue;

					Func<IPCRequest, Task<string>> handler = (Func<IPCRequest, Task<string>>)Delegate.CreateDelegate(typeof(Func<IPCRequest, Task<string>>), method);
					Register(mcpToolAttri.Method, handler);
				}
			}
		}

		/// <summary>
		/// 分发请求
		/// </summary>
		public async Task<IPCResponse> DispatchAsync(IPCRequest request)
		{
			if (!m_funcHandlers.TryGetValue(request.Method, out Func<IPCRequest, Task<string>> handler))
			{
				return new IPCResponse
				{
					Id = request.Id,
					Success = false,
					Error = new IPCError
					{
						Code = "METHOD_NOT_FOUND",
						Message = request.Method
					}
				};
			}

			try
			{
				string result = await handler(request);
				return new IPCResponse
				{
					Id = request.Id,
					Success = true,
					Result = result
				};
			}
			catch (Exception e)
			{
				return new IPCResponse
				{
					Id = request.Id,
					Success = false,
					Error = new IPCError
					{
						Code = "EXCEPTION",
						Message = e.Message
					}
				};
			}
		}
	}
}
