namespace Echo.Mcp.Protocol
{
	/// <summary>
	/// IPC 请求
	/// </summary>
	public class IPCRequest
	{
		/// <summary>
		/// 请求 ID
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// 请求方法
		/// </summary>
		public string Method { get; set; }

		/// <summary>
		/// 请求参数
		/// </summary>
		public string Params { get; set; }
	}
}
