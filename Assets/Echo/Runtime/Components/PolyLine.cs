using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Component
{
	/// <summary>
	/// 多段线组件
	/// </summary>
	public class PolyLine : MonoBehaviour
	{
		/// <summary>
		/// 多段线的所有顶点坐标列表
		/// </summary>
		[SerializeField]
		private List<Vector3> m_points = new List<Vector3>();
		public IReadOnlyList<Vector3> Points => m_points;

		/// <summary>
		/// 颜色
		/// </summary>
		[SerializeField]
		private Color m_color = Color.white;
		public Color Color
		{
			get => m_color;
			set => m_color = value;
		}

		/// <summary>
		/// 是否闭合
		/// </summary>
		[SerializeField]
		private bool m_isClosed = false;
		public bool Closed
		{
			get => m_isClosed;
			set => m_isClosed = value;
		}

		/// <summary>
		/// 添加点
		/// </summary>
		/// <param name="point"></param>
		public void AddPoint(Vector3 point)
		{
			m_points.Add(point);
		}

		/// <summary>
		/// 设置指定索引处的顶点坐标。
		/// </summary>
		/// <param name="index"></param>
		/// <param name="point"></param>
		public void SetPointAt(int index, Vector3 point)
		{
			if (index < 0 || index > m_points.Count)
				return;

			m_points[index] = point;
			if (m_isClosed)
			{
				if (index == 0)
					m_points[m_points.Count - 1] = point;
				if (index == m_points.Count - 1)
					m_points[0] = point;
			}
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
			if (index < 0 || index > m_points.Count)
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
				m_isClosed = false;
				Debug.LogWarning("PolyLine: 点数量不足，无法实现闭合。");
				return;
			}

			m_isClosed = isClosed;
		}

		private void OnDrawGizmos()
		{
			DrawPolyLine();
		}

		private void OnDrawGizmosSelected()
		{
			DrawPolyLine(true);
		}

		private void DrawPolyLine(bool isSelect = false)
		{
			if (m_points == null || m_points.Count < 2)
				return;

			//转成世界坐标
			Vector3[] worldPoints = new Vector3[m_points.Count];
			for (int i = 0; i < m_points.Count; i++)
			{
				worldPoints[i] = transform.position + m_points[i];
			}

			//绘制多段线
			float width = isSelect ? 2f : 1f;
			Handles.color = isSelect ? Color.cyan : m_color;
			Handles.DrawAAPolyLine(width, worldPoints);

			var lineRender = this.gameObject.GetComponent<LineRenderer>();
			lineRender.material = new Material(Shader.Find("Unlit/Color")) { color = Handles.color };
		}
	}
}
