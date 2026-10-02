using Echo.Mcp.Protocol;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Echo.Mcp.Bridge
{
	/// <summary>
	/// EchoKnight 通信桥接器
	/// </summary>
	public class EchoKnightBridge : IDisposable
	{
		/// <summary>
		/// Unity Editor 地址
		/// </summary>
		private readonly string m_host;

		/// <summary>
		/// Unity Editor IPC 端口
		/// </summary>
		private readonly int m_port;

		/// <summary>
		/// TCP 客户端
		/// </summary>
		private TcpClient? m_client;

		/// <summary>
		/// 消息长度字段字节数
		/// </summary>
		private const int LENGTH_PREFIX_SIZE = sizeof(int);

		/// <summary>
		/// JSON序列化设置
		/// </summary>
		private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		public EchoKnightBridge(string host = "127.0.0.1", int port = 9257)
		{
			m_host = host;
			m_port = port;
			m_client = null;
		}

		public void Dispose()
		{
			Disconnect();
		}

		/// <summary>
		/// 连接 EchoKnight
		/// </summary>
		public async Task ConnectAsync(CancellationToken cancellationToken = default)
		{
			if (m_client != null)
				return;
			m_client = new TcpClient();

			try
			{
				await m_client.ConnectAsync(m_host, m_port, cancellationToken);
			}
			catch
			{
				m_client.Dispose();
				m_client = null;

				throw;
			}
		}

		/// <summary>
		/// 断开 EchoKnight
		/// </summary>
		public void Disconnect()
		{
			m_client?.Dispose();
			m_client = null;
		}

		/// <summary>
		/// IPC 请求
		/// </summary>
		public async Task<IPCResponse?> RequestAsync(IPCRequest request, CancellationToken cancellationToken = default)
		{
			//发送请求
			await SendAsync(request);

			//接收响应
			IPCResponse? response = await ReceiveAsync();
			if (response == null)
				return null;

			//校验请求与响应
			if (response.Id != request.Id)
			{
				throw new InvalidOperationException($"IPC 响应 ID 不匹配，请求 ID: {request.Id}，响应 ID: {response.Id}");
			}

			return response;
		}

		/// <summary>
		/// 发送 IPC 消息
		/// </summary>
		private async Task SendAsync(IPCRequest request, CancellationToken cancellationToken = default)
		{
			if (m_client == null || !m_client.Connected)
				throw new InvalidOperationException("EchoKnight 未连接");

			//转为json字符串
			string json = JsonSerializer.Serialize(request, s_jsonOptions);

			byte[] payload = Encoding.UTF8.GetBytes(json);
			byte[] length = BitConverter.GetBytes(payload.Length);

			NetworkStream stream = m_client.GetStream();
			await stream.WriteAsync(length, cancellationToken);
			await stream.WriteAsync(payload, cancellationToken);
			await stream.FlushAsync(cancellationToken);
		}

		/// <summary>
		/// 接收 IPC 消息
		/// </summary>
		private async Task<IPCResponse?> ReceiveAsync(CancellationToken cancellationToken = default)
		{
			if (m_client == null || !m_client.Connected)
				throw new InvalidOperationException("EchoKnight 未连接");

			NetworkStream stream = m_client.GetStream();
			string? json = await ReadMessageAsync(stream, cancellationToken);
			if (json == null)
				return null;

			return JsonSerializer.Deserialize<IPCResponse>(json, s_jsonOptions);
		}

		/// <summary>
		/// 读取一条完整消息
		/// </summary>
		private static async Task<string?> ReadMessageAsync(NetworkStream stream, CancellationToken cancellationToken)
		{
			byte[] lengthBuffer = new byte[LENGTH_PREFIX_SIZE];
			bool success = await ReadExactAsync(stream, lengthBuffer, cancellationToken);
			if (!success)
				return null;

			int length = BitConverter.ToInt32(lengthBuffer, 0);
			if (length <= 0)
				throw new InvalidOperationException($"Invalid message length: {length}");

			byte[] payloadBuffer = new byte[length];
			success = await ReadExactAsync(stream, payloadBuffer, cancellationToken);
			if (!success)
				return null;

			return Encoding.UTF8.GetString(payloadBuffer);
		}

		/// <summary>
		/// 读取指定数量的字节
		/// </summary>
		private static async Task<bool> ReadExactAsync(NetworkStream stream, byte[] buffer, CancellationToken cancellationToken)
		{
			int offset = 0;
			while (offset < buffer.Length)
			{
				int count = await stream.ReadAsync(buffer, offset, buffer.Length - offset, cancellationToken);
				if (count == 0)
					return false;

				offset += count;
			}
			return true;
		}
	}
}
