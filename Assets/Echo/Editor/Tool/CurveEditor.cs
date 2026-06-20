using Echo.Component;
using System;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor.Tool
{
	[CustomEditor(typeof(Curve), true)]
	[CanEditMultipleObjects]
	public class CurveEditor : UnityEditor.Editor
	{
		private Curve m_curve = null;

		private void OnSceneGUI()
		{
			m_curve = (Curve)target;
			if (m_curve == null)
				return;

			switch (m_curve.Type)
			{
				case CurveType.Arc:
					break;
				case CurveType.PolyLine:
					//绘制多段线
					DrawPolyLine();
					//绘制控制点
					DrawPolyLineCtrlPoint();
					break;
				case CurveType.Circle:
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// 绘制多段线
		/// </summary>
		private void DrawPolyLine()
		{
			PolyLine polyLine = (PolyLine)m_curve;
			if (polyLine.Points == null || polyLine.Points.Count < 2)
				return;

			int count = polyLine.Points.Count;
			Handles.color = Color.cyan;
			for (int i = 0; i < count - 1; i++)
			{
				Vector3 pt1 = polyLine.transform.TransformPoint(polyLine.Points[i]);
				Vector3 pt2 = polyLine.transform.TransformPoint(polyLine.Points[i + 1]);

				Handles.DrawAAPolyLine(m_curve.Width + 1.0f, pt1, pt2);
			}

			if (polyLine.Closed)
			{
				Vector3 pt1 = polyLine.transform.TransformPoint(polyLine.Points[count - 1]);
				Vector3 pt2 = polyLine.transform.TransformPoint(polyLine.Points[0]);

				Handles.DrawAAPolyLine(polyLine.Width + 1.0f, pt1, pt2);
			}
		}

		/// <summary>
		/// 绘制多段线控制点
		/// </summary>
		private void DrawPolyLineCtrlPoint()
		{
			PolyLine polyLine = (PolyLine)m_curve;
			if (polyLine.Points == null || polyLine.Points.Count < 2)
				return;

			for (int i = 0; i < polyLine.Points.Count; i++)
			{
				//转成世界坐标
				Vector3 worldPos = polyLine.transform.TransformPoint(polyLine.Points[i]);
				float size = HandleUtility.GetHandleSize(worldPos) * 0.08f;
				Vector3 snap = Vector3.one * 0.5f;

				EditorGUI.BeginChangeCheck();
				Vector3 newWorldPos = Handles.FreeMoveHandle(worldPos, size, snap, Handles.SphereHandleCap);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(m_curve, "Move PolyLine Point");
					//转成模型坐标
					Vector3 pt = polyLine.transform.InverseTransformPoint(newWorldPos);
					polyLine.SetPointAt(i, pt);
					EditorUtility.SetDirty(polyLine);
				}
			}
		}
	}
}
