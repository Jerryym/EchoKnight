using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
	/// <summary>
	/// 捕捉点接口类
	/// </summary>
	public interface ISnapPoint
	{
		/// <summary>
		/// 获取捕捉点
		/// </summary>
		public void GetSnapPoints(List<Vector3> points);
	}
}
