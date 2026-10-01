using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace Echo.MCP
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			//创建应用程序宿主构建器
			HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

			//配置控制台日志
			builder.Logging.AddConsole(options =>
			{
				options.LogToStandardErrorThreshold = LogLevel.Trace;
			});

			//注册并配置 MCP Server
			builder.Services.AddMcpServer() //注册 MCP Server 服务
				.WithStdioServerTransport() //使用stdio作为 MCP 通信传输层
				.WithToolsFromAssembly();   //扫描并注册当前程序集中的 MCP Tools

			//构建应用程序宿主并启动 MCP Server
			await builder.Build().RunAsync();
		}
	}
}
