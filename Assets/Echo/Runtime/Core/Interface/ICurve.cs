using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
	/// <summary>
	/// 曲线接口类
	/// </summary>
	public interface ICurve
	{
		/// <summary>
		/// 曲线类型
		/// </summary>
		CurveType Type { get; }

		/// <summary>
		/// 获取曲线长度
		/// </summary>
		float GetLength();
		
		/// <summary>
		/// 起点
		/// </summary>
		Vector3 StartPoint();
		/// <summary>
		/// 终点
		/// </summary>
		Vector3 EndPoint();
		
		/// <summary>
		/// 根据插值参数获取曲线上的三维坐标点
		/// </summary>
		/// <param name="t">曲线参数（0 为起点，1 为终点）</param>
		Vector3 GetPoint(float t);
		/// <summary>
		/// 根据指定的间距（步长）对曲线进行等距采样，获取点集
		/// </summary>
		/// <param name="spacing">步长间隔，若为-1，则返回曲线的默认采样点集</param>
		IReadOnlyList<Vector3> GetPoints(float spacing = -1);
	}
}
