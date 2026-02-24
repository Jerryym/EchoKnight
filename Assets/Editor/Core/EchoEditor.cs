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
	/// 命令信息
	/// </summary>
	public class CommandInfo
	{
		/// <summary>
		/// 命令组名
		/// </summary>
		public string group;
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
