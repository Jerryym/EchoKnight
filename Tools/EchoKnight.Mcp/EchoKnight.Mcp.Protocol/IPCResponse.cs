namespace Echo.Mcp.Protocol
{
	/// <summary>
	/// IPC 响应消息
	/// </summary>
	public class IPCResponse
	{
		/// <summary>
		/// 请求 ID
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// 请求是否成功执行
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// 结果
		/// </summary>
		public string Result { get; set; }

		/// <summary>
		/// 错误信息
		/// </summary>
		public IPCError Error { get; set; } 
	}
}
