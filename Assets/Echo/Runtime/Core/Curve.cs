using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
	/// <summary>
	/// 曲线基类
	/// </summary>
	public abstract class Curve : MonoBehaviour, ICurve
	{
		public abstract CurveType Type { get; }

		public abstract float Width { get; set; }
		public abstract Color Color { get; set; }

		public abstract float GetLength();
		public abstract Vector3 StartPoint();
		public abstract Vector3 EndPoint();

		public abstract Vector3 GetPoint(float t);
		public abstract IReadOnlyList<Vector3> GetPoints(float spacing = -1);
	}

}
