namespace Echo.Mcp.Protocol
{
	/// <summary>
	/// IPC 错误信息
	/// </summary>
	public class IPCError
	{
		/// <summary>
		/// 错误码
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 错误信息
		/// </summary>
		public string Message { get; set; }
	}
}
