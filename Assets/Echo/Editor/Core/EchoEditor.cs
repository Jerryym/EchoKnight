using System.Collections.Generic;

namespace Echo.Editor
{
	public enum TerrainType
	{
		None = -1,
		Plain,      //平原
		Hill,       //丘陵
		Mountain,   //山地
	}

	/// <summary>
	/// 工具状态
	/// </summary>
	public enum ToolState
	{
		Idle,
		Running,
		Completed,
		Cancelled
	}

	/// <summary>
	/// 命令组
	/// </summary>
	public class CommandGroup
	{
		/// <summary>
		/// 命令组名
		/// </summary>
		public string name;
		public List<CommandInfo> commands = new List<CommandInfo>();
	}

	/// <summary>
	/// 命令信息
	/// </summary>
	public class CommandInfo
	{
		/// <summary>
		/// 命令ID
		/// </summary>
		public string id;
		/// <summary>
		/// 命令名称
		/// </summary>
		public string name;
	}
}
