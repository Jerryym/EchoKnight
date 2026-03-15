using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Component
{
	/// <summary>
	/// 多段线组件
	/// </summary>
	public class PolyLine : MonoBehaviour, ISelection
	{
		/// <summary>
		/// 多段线的所有顶点坐标列表
		/// </summary>
		[SerializeField]
		private List<Vector3> m_points = new List<Vector3>();
		public IReadOnlyList<Vector3> Points => m_points;

		/// <summary>
		/// 线宽
		/// </summary>
		[SerializeField]
		private float m_width = 1.5f;
		public float Width
		{
			get { return m_width; }
			set { m_width = value; }
		}

		/// <summary>
		/// 颜色
		/// </summary>
		[SerializeField]
		private Color m_color = Color.white;
		public Color Color
		{
			get { return m_color; }
			set { m_color = value; }
		}

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
			SelectionManager.Unregister(this.gameObject);
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
			Handles.color = m_color;
			Handles.DrawAAPolyLine(m_width, worldPoints);

			if (m_isClosed)
			{
				Vector3 pt1 = worldPoints[worldPoints.Length - 1];
				Vector3 pt2 = worldPoints[0];
				Handles.DrawAAPolyLine(m_width, pt1, pt2);
			}
		}
	}
}
