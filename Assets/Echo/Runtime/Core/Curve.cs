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

		public virtual Vector3 WorldStartPoint()
		{
			return transform.TransformPoint(StartPoint());
		}

		public virtual Vector3 WorldEndPoint()
		{
			return transform.TransformPoint(EndPoint());
		}

		public virtual Vector3 GetWorldPoint(float t)
		{
			return transform.TransformPoint(GetPoint(t));
		}

		public virtual IReadOnlyList<Vector3> GetWorldPoints(float spacing = -1)
		{
			var points = GetPoints(spacing);
			List<Vector3> worldPts = new List<Vector3>(points.Count);
			for (int i = 0; i < points.Count; i++)
			{
				worldPts.Add(transform.TransformPoint(points[i]));
			}
			return worldPts;
		}
	}
}
