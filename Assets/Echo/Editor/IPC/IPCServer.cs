using Echo.Mcp.Protocol;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Echo.Editor.IPC
{
	/// <summary>
	/// IPC 服务器
	/// </summary>
	public class IPCServer : IDisposable
	{
		/// <summary>
		/// 消息长度字段字节数
		/// </summary>
		private const int LENGTH_PREFIX_SIZE = sizeof(int);

		/// <summary>
		/// 端口
		/// </summary>
		private readonly int m_port = 9257;

		/// <summary>
		/// TCP 监听器
		/// </summary>
		private TcpListener m_listener;
		/// <summary>
		/// 监听任务
		/// </summary>
		private Task m_listenTask;

		private CancellationTokenSource m_cancellationTokenSource;

		private RequestDispatcher m_dispatcher;

		/// <summary>
		/// JSON 序列化配置
		/// </summary>
		private static readonly JsonSerializerSettings s_jsonOptions = new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		};

		public IPCServer(RequestDispatcher dispatcher, int port = 9257)
		{
			m_dispatcher = dispatcher;
			m_port = port;
		}

		public void Dispose()
		{
			Stop();
		}

		/// <summary>
		/// 启动服务器
		/// </summary>
		public void Start()
		{
			if (m_listener != null)
				return;

			m_cancellationTokenSource = new CancellationTokenSource();

			m_listener = new TcpListener(IPAddress.Loopback, m_port);
			m_listener.Start();

			m_listenTask = ListenAsync(m_cancellationTokenSource.Token);

			Debug.Log($"IPC Server started: 127.0.0.1:{m_port}");
		}

		/// <summary>
		/// 停止服务器
		/// </summary>
		public void Stop()
		{
			m_cancellationTokenSource?.Cancel();

			m_listener?.Stop();
			m_listener = null;

			m_cancellationTokenSource?.Dispose();
			m_cancellationTokenSource = null;

			m_listenTask = null;
		}

		/// <summary>
		/// 监听客户端连接
		/// </summary>
		private async Task ListenAsync(CancellationToken cancellationToken)
		{
			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					TcpClient client = await m_listener.AcceptTcpClientAsync();
					Debug.Log("EchoKnight IPC client connected.");
					await HandleClientAsync(client, cancellationToken);
				}
			}
			catch (ObjectDisposedException)
			{
				// Server stopped.
			}
			catch (SocketException)
			{
				if (!cancellationToken.IsCancellationRequested)
					throw;
			}
		}

		/// <summary>
		/// 处理客户端连接
		/// </summary>
		private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
		{
			using (client)
			{
				try
				{
					NetworkStream stream = client.GetStream();
					while (!cancellationToken.IsCancellationRequested)
					{
						IPCRequest request = await ReadMessageAsync(stream, cancellationToken);
						if (request == null)
							break;

						string requestStr = JsonConvert.SerializeObject(request, Formatting.None, s_jsonOptions);
						Debug.Log($"EchoKnight IPC received: {requestStr}");

						IPCResponse response = await m_dispatcher.DispatchAsync(request);
						await WriteMessageAsync(stream, response, cancellationToken);
					}
				}
				catch (OperationCanceledException)
				{
					// Server stopped.
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}

		/// <summary>
		/// 读取一条完整消息
		/// </summary>
		private static async Task<IPCRequest> ReadMessageAsync(NetworkStream stream, CancellationToken cancellationToken)
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

			string json = Encoding.UTF8.GetString(payloadBuffer);
			return JsonConvert.DeserializeObject<IPCRequest>(json, s_jsonOptions);
		}

		/// <summary>
		/// 从数据流中读取指定数量的字节
		/// </summary>
		private static async Task<bool> ReadExactAsync(NetworkStream stream, byte[] buffer, CancellationToken cancellationToken)
		{
			int offset = 0;
			while (offset < buffer.Length)
			{
				int count = await stream.ReadAsync(buffer, offset,buffer.Length - offset, cancellationToken);
				if (count == 0)
					return false;

				offset += count;
			}
			return true;
		}

		/// <summary>
		/// 写入一条完整消息
		/// </summary>
		private static async Task WriteMessageAsync(NetworkStream stream, IPCResponse response, CancellationToken cancellationToken)
		{
			string json = JsonConvert.SerializeObject(response, Formatting.None, s_jsonOptions);

			byte[] payload = Encoding.UTF8.GetBytes(json);
			byte[] length = BitConverter.GetBytes(payload.Length);

			await stream.WriteAsync(length, 0, length.Length, cancellationToken);
			await stream.WriteAsync(payload, 0, payload.Length, cancellationToken);
			await stream.FlushAsync(cancellationToken);
		}
	}
}
