using Echo.Component;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor.Tool
{
	[CustomEditor(typeof(Curve), true)]
	[CanEditMultipleObjects]
	public class CurveEditor : UnityEditor.Editor
	{
		private Curve m_curve = null;

		private static readonly Color SELECT_COLOR = Color.cyan;

		private void OnSceneGUI()
		{
			m_curve = (Curve)target;
			if (m_curve == null)
				return;

			switch (m_curve.Type)
			{
				case CurveType.Arc:
					//绘制弧线
					DrawArc();
					//绘制控制点
					DrawArcCtrlPoint();
					break;
				case CurveType.PolyLine:
					//绘制多段线
					DrawPolyLine();
					//绘制控制点
					DrawPolyLineCtrlPoint();
					break;
				case CurveType.Circle:
					DrawCircle();
					DrawCircleCtrlPoint();
					break;
				default:
					break;
			}
		}

		private void DrawArc()
		{
			Arc arc = (Arc)m_curve;
			if (arc == null)
				return;

			var transform = arc.transform;
			Vector3 centerPt = arc.WorldCenterPoint;
			Vector3 startDir = arc.WorldStartDirection;
			float radius = arc.Radius;
			float sweepAngle = Mathf.Abs(arc.SweepAngle) * Mathf.Rad2Deg;

			Handles.color = SELECT_COLOR;
			Handles.DrawSolidDisc(centerPt, transform.up, 0.08f);
			Handles.DrawWireArc(centerPt, transform.up, startDir, sweepAngle, radius);
		}

		private void DrawArcCtrlPoint()
		{
			Arc arc = (Arc)m_curve;
			if (arc == null)
				return;

			var transform = arc.transform;
			Vector3 centerPt = arc.WorldCenterPoint;
			float size = HandleUtility.GetHandleSize(centerPt) * 0.08f;

			Handles.color = SELECT_COLOR;
			Handles.DrawSolidDisc(centerPt, transform.up, size);

			Vector3 startPt = arc.WorldStartPoint();
			Handles.DrawSolidDisc(startPt, transform.up, size);

			Vector3 endPt = arc.WorldEndPoint();
			Handles.DrawSolidDisc(endPt, transform.up, size);
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
			Handles.color = SELECT_COLOR;
			for (int i = 0; i < count - 1; i++)
			{
				Vector3 pt1 = polyLine.transform.TransformPoint(polyLine.Points[i]);
				Vector3 pt2 = polyLine.transform.TransformPoint(polyLine.Points[i + 1]);

				Handles.DrawAAPolyLine(m_curve.width + 1.0f, pt1, pt2);
			}

			if (polyLine.Closed)
			{
				Vector3 pt1 = polyLine.transform.TransformPoint(polyLine.Points[count - 1]);
				Vector3 pt2 = polyLine.transform.TransformPoint(polyLine.Points[0]);

				Handles.DrawAAPolyLine(polyLine.width + 1.0f, pt1, pt2);
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

			var worldPoints = polyLine.GetWorldPoints();
			for (int i = 0; i < worldPoints.Count; i++)
			{
				//转成世界坐标
				Vector3 worldPos = worldPoints[i];
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

		private void DrawCircle()
		{
			Circle circle = (Circle)m_curve;
			if (circle == null)
				return;

			Handles.color = SELECT_COLOR;
			Vector3 centerPt = circle.WorldCenterPoint;
			Handles.DrawWireDisc(centerPt, Vector3.up, circle.Radius);
		}

		private void DrawCircleCtrlPoint()
		{
			Circle circle = (Circle)m_curve;
			if (circle == null)
				return;

			Vector3 centerPt = circle.WorldCenterPoint;
			float size = HandleUtility.GetHandleSize(centerPt) * 0.08f;

			Handles.color = SELECT_COLOR;
			Handles.DrawSolidDisc(centerPt, circle.transform.up, size);
		}
	}
}
