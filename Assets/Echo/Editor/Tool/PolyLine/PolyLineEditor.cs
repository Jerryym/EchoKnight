using Echo.Component;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor.Tool
{
	[CustomEditor(typeof(PolyLine))]
	[CanEditMultipleObjects]
	public class PolyLineEditor : UnityEditor.Editor
	{
		private PolyLine m_polyLine = null;

		private void OnSceneGUI()
		{
			m_polyLine = (PolyLine)target;
			if (m_polyLine.Points == null || m_polyLine.Points.Count < 2)
				return;

			//绘制多段线
			DrawPolyLine();
			//绘制控制点
			DrawControlPoints();
		}

		/// <summary>
		/// 绘制多段线
		/// </summary>
		private void DrawPolyLine()
		{
			int count = m_polyLine.Points.Count;

			Handles.color = Color.cyan;
			for (int i = 0; i < count - 1; i++)
			{
				Vector3 pt1 = m_polyLine.transform.TransformPoint(m_polyLine.Points[i]);
				Vector3 pt2 = m_polyLine.transform.TransformPoint(m_polyLine.Points[i + 1]);

				Handles.DrawAAPolyLine(m_polyLine.Width + 1.0f, pt1, pt2);
			}

			if (m_polyLine.Closed)
			{
				Vector3 pt1 = m_polyLine.transform.TransformPoint(m_polyLine.Points[count - 1]);
				Vector3 pt2 = m_polyLine.transform.TransformPoint(m_polyLine.Points[0]);

				Handles.DrawAAPolyLine(m_polyLine.Width + 1.0f, pt1, pt2);
			}
		}

		/// <summary>
		/// 绘制控制点
		/// </summary>
		private void DrawControlPoints()
		{
			for (int i = 0; i < m_polyLine.Points.Count; i++)
			{
				//转成世界坐标
				Vector3 worldPos = m_polyLine.transform.TransformPoint(m_polyLine.Points[i]);
				float size = HandleUtility.GetHandleSize(worldPos) * 0.08f;
				Vector3 snap = Vector3.one * 0.5f;

				EditorGUI.BeginChangeCheck();
				Vector3 newWorldPos = Handles.FreeMoveHandle(worldPos, size, snap, Handles.SphereHandleCap);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(m_polyLine, "Move PolyLine Point");
					//转成模型坐标
					Vector3 pt = m_polyLine.transform.InverseTransformPoint(newWorldPos);
					m_polyLine.SetPointAt(i, pt);
					EditorUtility.SetDirty(m_polyLine);
				}
			}
		}
	}
}
