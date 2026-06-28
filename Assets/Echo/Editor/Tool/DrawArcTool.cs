using System.Collections.Generic;
using Echo.Component;
using Echo.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor.Tool
{
	public class DrawArcTool : IEditorTool
	{
		private ToolState m_state = ToolState.Idle;

		/// <summary>
		/// 圆弧点数组: 0-圆心 1-起始点 2-结束点
		/// </summary>
		private List<Vector3> m_points = null;
		/// <summary>
		/// 预览点
		/// </summary>
		private Vector3 m_previewPt = Vector3.zero;

		/// <summary>
		/// 当前活动SceneView
		/// </summary>
		private SceneView m_sceneView = null;

		private const string m_tag = "Arc";

		public DrawArcTool()
		{
			m_points = new List<Vector3>();
			m_sceneView = RPGEditorToolWindow.ActiveWindow.SceneView;
		}

		void IEditorTool.Activate()
		{
			m_state = ToolState.Running;
		}

		void IEditorTool.DeActivate()
		{
			m_points.Clear();
			m_state = ToolState.Idle;
		}

		void IEditorTool.OnSceneGUI(SceneView sceneView)
		{
			if (m_state != ToolState.Running)
				return;

			Event e = Event.current;
			HandleInput(e);
			DrawPreview();
		}

		private void HandleInput(Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown://鼠标按下
					{
						if (e.button == 0)//左键
						{
							AddPoint();
							e.Use();
						}
						else if (e.button == 1)//右键
						{
							Cancel();
							e.Use();
						}
					}
					break;
				case EventType.MouseMove://鼠标移动
					{
						UpdatePreview(e.mousePosition);
						break;
					}
				case EventType.KeyDown:
					{
						if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Escape || e.keyCode == KeyCode.KeypadEnter)
						{
							Finish();
							e.Use();
						}
						break;
					}
				default:
					break;
			}
		}

		private void DrawPreview()
		{
			if (m_points.Count == 0)
				return;

			Handles.color = Color.yellow;
			//绘制控制点
			for (int i = 0; i < m_points.Count; i++)
			{
				Vector3 pt = m_points[i];
				Handles.DrawSolidDisc(pt, Vector3.up, 0.1f);
			}

			if (m_points.Count == 1)//绘制半径预览线
			{
				Vector3 centerPt = m_points[0];
				Handles.DrawLine(centerPt, m_previewPt);
			}
			else if (m_points.Count == 2)//绘制圆弧预览
			{
				Vector3 centerPt = m_points[0];
				Vector3 startPt = m_points[1];
				Vector3 endPt = m_previewPt;

				Vector3 startDir = (startPt - centerPt).normalized;
				Vector3 endDir = (endPt - centerPt).normalized;

				float radius = Vector3.Distance(centerPt, startPt);
				float sweepAngle = Vector3.SignedAngle(startDir, endDir, Vector3.up);
				Handles.DrawLine(centerPt, startPt);
				Handles.DrawWireArc(centerPt, Vector3.up, startDir, sweepAngle, radius);
			}
			m_sceneView.Repaint();
		}

		private void AddPoint()
		{
			if (m_points.Count >= 3)
				return;

			m_points.Add(m_previewPt);
			if (m_points.Count == 3)
				Finish();
		}

		private void UpdatePreview(Vector2 position)
		{
			Plane plane = new Plane(Vector3.up, Vector3.zero);
			if (EditorTool.HitPosition(plane, position, out Vector3 hitPt))
			{
				m_previewPt = hitPt;
			}
			else
			{
				m_previewPt = new Vector3(position.x, 0f, position.y);
			}
		}

		/// <summary>
		/// 确定
		/// </summary>
		private void Finish()
		{
			m_state = ToolState.Completed;
			if (m_points.Count < 3)
			{
				EditorToolManager.ClearTool();
				return;
			}

			if (!Utils.EditorTool.TagExist(m_tag))
				Utils.EditorTool.AddTag(m_tag);

			GameObject go = new GameObject("圆弧");
			go.tag = m_tag;
			Undo.RegisterCreatedObjectUndo(go, "Draw Arc");

			//创建圆弧组件
			var arc = CreateArc(go);
			PickManager.Register(go, arc);

			EditorToolManager.ClearTool();
		}

		/// <summary>
		/// 取消
		/// </summary>
		private void Cancel()
		{
			m_state = ToolState.Cancelled;
			EditorToolManager.ClearTool();
		}

		private IPickable CreateArc(GameObject go)
		{
			var arcComponent = go.AddComponent<Arc>();
			arcComponent.SetFromPoints(m_points[0], m_points[1], m_points[2]);
			return arcComponent;
		}
	}
}
