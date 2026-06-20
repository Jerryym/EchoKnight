using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Component
{
	/// <summary>
	/// 多段线组件
	/// </summary>
	public class PolyLine : Curve, IPickable
	{
		/// <summary>
		/// 多段线的所有顶点坐标列表
		/// </summary>
		[SerializeField]
		private List<Vector3> m_points = new List<Vector3>();
		public IReadOnlyList<Vector3> Points => m_points;

		/// <summary>
		/// 是否闭合
		/// </summary>
		[SerializeField]
		private bool m_isClosed = false;
		public bool Closed
		{
			get { return m_isClosed; }
			set { m_isClosed = value; }
		}

		public override CurveType Type => CurveType.PolyLine;
		public override float Width { get; set; } = 1.5f;
		public override Color Color { get; set; } = Color.cyan;

		/// <summary>
		/// 添加点
		/// </summary>
		public void AddPoint(Vector3 point)
		{
			m_points.Add(point);
		}

		/// <summary>
		/// 设置指定索引处的顶点坐标。
		/// </summary>
		public void SetPointAt(int index, Vector3 point)
		{
			if (index < 0 || index >= m_points.Count)
				return;

			m_points[index] = point;
		}

		/// <summary>
		/// 插入点
		/// </summary>
		public void InsertPoint(int index, Vector3 point)
		{
			if (index < 0 || index > m_points.Count)
				return;
			
			m_points.Insert(index, point);
		}

		/// <summary>
		/// 移除指定索引处的顶点
		/// </summary>
		public void RemovePoint(int index)
		{
			if (index < 0 || index >= m_points.Count)
				return;

			m_points.RemoveAt(index);
		}

		/// <summary>
		/// 清空
		/// </summary>
		public void Clear()
		{
			m_points.Clear();
			m_isClosed = false;
		}

		/// <summary>
		/// 设置多段线的闭合状态
		/// </summary>
		/// <param name="isClosed">是否闭合</param>
		public void SetClosed(bool isClosed)
		{
			if (m_points.Count < 2)
			{
				Debug.LogWarning("PolyLine: 点数量不足，无法实现闭合。");
				m_isClosed = false;
				return;
			}

			m_isClosed = isClosed;
		}

		public float HitObject(Vector2 hitPt)
		{
			if (m_points == null || m_points.Count < 2)
				return float.MaxValue;

			List<Vector3> points = new List<Vector3>();
			foreach (var pt in m_points)
			{
				points.Add(transform.TransformPoint(pt));
			}
			return HandleUtility.DistanceToPolyLine(points.ToArray());
		}

		public GameObject GetGameObject()
		{
			return this.gameObject;
		}

		private void OnDestroy()
		{
			PickManager.Unregister(this.gameObject);
		}

		private void OnDrawGizmos()
		{
			DrawPolyLine();
		}

		/// <summary>
		/// 绘制多段线
		/// </summary>
		private void DrawPolyLine()
		{
			if (m_points == null || m_points.Count < 2)
				return;

			//转成世界坐标
			Vector3[] worldPoints = new Vector3[m_points.Count];
			for (int i = 0; i < m_points.Count; i++)
			{
				worldPoints[i] = transform.TransformPoint(m_points[i]);
			}

			//绘制多段线
			Handles.color = Color;
			Handles.DrawAAPolyLine(Width, worldPoints);

			if (m_isClosed)
			{
				Vector3 pt1 = worldPoints[worldPoints.Length - 1];
				Vector3 pt2 = worldPoints[0];
				Handles.DrawAAPolyLine(Width, pt1, pt2);
			}
		}

		public override float GetLength()
		{
			if (m_points == null || m_points.Count < 2)
				return 0.0f;

			float length = 0.0f;
			for (int i = 0; i < m_points.Count - 1; i++)
			{
				length += Vector3.Distance(m_points[i], m_points[i + 1]);
			}

			//闭合，计算多计算一段
			if (m_isClosed)
				length += Vector3.Distance(m_points[0], m_points[m_points.Count - 1]);

			return length;
		}

		public override Vector3 StartPoint()
		{
			return m_points[0];
		}

		public override Vector3 EndPoint()
		{
			return m_points[m_points.Count - 1];
		}

		public override Vector3 GetPoint(float t)
		{
			if (m_points == null || m_points.Count < 2)
				return Vector3.zero;

			//归一化
			t = Mathf.Clamp01(t);

			//获取曲线长度
			float length = GetLength();
			float targetLength = length * t;

			float sum = 0.0f;
			for (int i = 0; i < m_points.Count - 1; i++)
			{
				Vector3	pt0 = m_points[i];
				Vector3 pt1 = m_points[i + 1];

				float distance = Vector3.Distance(pt0, pt1);
				if (distance <= 0)
					continue;

				if (sum + distance >= targetLength)
				{
					float lerpT = (targetLength - sum) / distance;
					return Vector3.Lerp(pt0, pt1, lerpT);
				}
				sum += distance;
			}
			return m_points[m_points.Count - 1];
		}

		public override IReadOnlyList<Vector3> GetPoints(float spacing = -1)
		{
			if (spacing == -1)
				return m_points;

			List<Vector3> result = new List<Vector3>();
			if (m_points == null || m_points.Count == 0)
				return result;

			if (m_points.Count == 1)
			{
				result.Add(m_points[0]);
				return result;
			}

			float sum = 0.0f;
			float sampleDistance = 0.0f;

			result.Add(m_points[0]);
			for (int i = 0; i < m_points.Count - 1; i++)
			{
				Vector3 pt0 = m_points[i];
				Vector3 pt1 = m_points[i + 1];

				float distance = Vector3.Distance(pt0, pt1);
				if (distance <= 0)
					continue;

				while (sum + distance >= sampleDistance)
				{
					float remain = sampleDistance - sum;
					float lerpT = remain / distance;

					Vector3 point = Vector3.Lerp(pt0, pt1, lerpT);
					result.Add(point);

					sampleDistance += spacing;
				}

				sum += distance;
			}

			//添加终点
			Vector3 endPt = m_points[m_points.Count - 1];
			if (result[result.Count - 1].Equals(endPt) != true)
				result.Add(endPt);

			return result;
		}
	}
}
