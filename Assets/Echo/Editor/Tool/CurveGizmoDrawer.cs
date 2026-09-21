using Echo.Component;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor.Tool
{
	/// <summary>
	/// 曲线绘制工具
	/// </summary>
	internal class CurveGizmoDrawer
	{
		/// <summary>
		/// 绘制多段线
		/// </summary>
		/// <param name="polyLine">多段线组件</param>
		/// <param name="gizmoType">Gizmo类型</param>
		[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
		private static void DrawPolyLine(PolyLine polyLine, GizmoType gizmoType)
		{
			if (polyLine == null)
			{
				Debug.LogError("多段线组件为空!");
				return;
			}

			if (polyLine.Points == null || polyLine.Points.Count < 2)
			{
				Debug.LogError("多段线点集长度小于2");
				return;
			}

			//获取多段线世界坐标
			List<Vector3> Pts = new List<Vector3>(polyLine.GetWorldPoints());
			if (polyLine.Closed)
				Pts.Add(Pts[0]);

			//绘制多段线
			Handles.color = polyLine.color;
			Handles.DrawAAPolyLine(polyLine.width, Pts.ToArray());
		}

		/// <summary>
		/// 绘制圆弧
		/// </summary>
		/// <param name="arc">圆弧组件</param>
		/// <param name="gizmoType">Gizmo类型</param>
		[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
		private static void DrawArc(Arc arc, GizmoType gizmoType)
		{
			if (arc == null)
			{
				Debug.LogError("圆弧组件为空!");
				return;
			}

			Transform transform = arc.transform;
			Vector3 center = arc.WorldCenterPoint;
			Vector3 from = arc.WorldStartDirection;
			float sweepAngle = -arc.SweepAngle * Mathf.Rad2Deg;

			//绘制圆弧
			Handles.color = arc.color;
			Handles.DrawWireArc(center, arc.transform.up, from, sweepAngle, arc.Radius);
		}

		/// <summary>
		/// 绘制圆
		/// </summary>
		/// <param name="circle">圆组件</param>
		/// <param name="gizmoType">Gizmo类型</param>
		[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
		private static void DrawCircle(Circle circle, GizmoType gizmoType)
		{
			if (circle == null)
			{
				Debug.LogError("圆组件为空!");
				return;
			}

			//绘制圆
			Handles.color = circle.color;
			Handles.DrawWireDisc(circle.WorldCenterPoint, circle.transform.up, circle.Radius);
		}
	}
}
