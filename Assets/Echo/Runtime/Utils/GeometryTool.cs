using System.Collections.Generic;
using UnityEngine;

namespace Echo.Utils
{
	/// <summary>
	/// 几何工具类
	/// </summary>
	public static class GeometryTool
	{
		/// <summary>
		/// 获取最接近的点
		/// </summary>
		public static bool GetClosestPoint(List<Vector3> points, in Vector3 basePt, out Vector3 targetPt)
		{
			targetPt = Vector3.zero;
			float minDistance = float.MaxValue;
			bool isFind = false;

			foreach (var pt in points)
			{
				float distance = Vector3.Distance(pt, basePt);
				if (distance < minDistance)
				{
					minDistance = distance;
					targetPt = pt;
					isFind = true;
				}
			}
			return isFind;
		}

		/// <summary>
		/// 判断多段线是否闭合
		/// </summary>
		public static bool IsPolyLineClosed(List<Vector3> points)
		{
			if (points == null || points.Count < 3)
				return false;

			Vector3 firstPt = points[0];
			Vector3 lastPt = points[points.Count - 1];
			return Vector3.Distance(firstPt, lastPt) < 1e-3f;
		}

		/// <summary>
		/// 过滤重复点
		/// </summary>
		public static void FilterDuplicatePoints(List<Vector3> points, float threshold = 1e-3f)
		{
			if (points == null || points.Count < 2)
				return;

			int count = 0;
			for (int i = 0; i < points.Count; i++)
			{
				Vector3 pt = points[i];
				bool isFind = false;
				for (int j = 0; j < count; j++)
				{
					if ((pt - points[j]).sqrMagnitude < threshold * threshold)
					{
						isFind = true;
						break;
					}
				}

				if (!isFind)
				{
					points[count] = pt;
					count++;
				}
			}

			points.RemoveRange(count, points.Count - count);
		}
	}
}
