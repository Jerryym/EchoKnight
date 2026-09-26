using System;

namespace Echo
{
	/// <summary>
	/// 程序化布设参数
	/// </summary>
	[Serializable]
	public abstract class PlacementParam
	{
		/// <summary>
		/// 布设方式
		/// </summary>
		public PlacementMode mode;
	}
}
