using UnityEngine;

namespace Echo
{
	/// <summary>
	/// 沿线布设参数
	/// </summary>
	public class LinePlacementParam : PlacementParam
	{
		/// <summary>
		/// 间距
		/// </summary>
		public float spacing = 1.0f;
		/// <summary>
		/// 起点偏移
		/// </summary>
		public float offsetStart = 0.0f;
		/// <summary>
		/// 终点偏移
		/// </summary>
		public float offsetEnd = 0.0f;
		/// <summary>
		/// 随机旋转
		/// </summary>
		public bool randomRotation = false;
		/// <summary>
		/// 旋转角度
		/// </summary>
		public float rotation = 0.0f;
		/// <summary>
		/// 随机旋转角度范围
		/// </summary>
		public Vector2 rotationRange = new Vector2(0f, 360f);
	}
}
