using Echo.Component;
using Echo.Editor.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor
{
	public class DrawCircleTool : IEditorTool
	{
		private ToolState m_state = ToolState.Idle;

		private SceneView m_sceneView = null;

		private const string m_tag = "Circle";

		private List<Vector3> m_points = null;

		private Vector3 m_previewPt = Vector3.negativeInfinity;

		public DrawCircleTool()
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
				case EventType.MouseDown:
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
						break;
					}
				case EventType.MouseMove:
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

		private void AddPoint()
		{
			if (m_points.Count > 2)
				return;

			m_points.Add(m_previewPt);
			if (m_points.Count == 2)
				Finish();
		}

		private void DrawPreview()
		{
			if (m_points.Count == 0)
				return;

			Handles.color = Color.yellow;
			for (int i = 0; i < m_points.Count; i++)
			{
				Vector3 pt = m_points[i];
				Handles.DrawSolidDisc(pt, Vector3.up, 0.1f);
			}

			//绘制半径预览线
			Vector3 centerPt = m_points[0];
			Handles.DrawLine(centerPt, m_previewPt);

			//绘制圆
			float radius = Vector3.Distance(m_previewPt, centerPt);
			Handles.DrawWireDisc(centerPt, Vector3.up, radius);

			m_sceneView.Repaint();
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

		private void Finish()
		{
			m_state = ToolState.Completed;
			if (m_points.Count < 2)
			{
				EditorToolManager.ClearTool();
				return;
			}

			if (!EditorTool.TagExist(m_tag))
				EditorTool.AddTag(m_tag);

			GameObject go = new GameObject("圆");
			go.tag = m_tag;
			Undo.RegisterCreatedObjectUndo(go, "Draw Circle");

			var circle = CreateCircle(go);
			PickManager.Register(go, circle);

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

		private IPickable CreateCircle(GameObject go)
		{
			if (m_points.Count < 2)
				return null;

			var circleComponent = go.AddComponent<Circle>();

			go.transform.position = m_points[0];
			circleComponent.CenterPoint = Vector3.zero;
			circleComponent.Radius = Vector3.Distance(m_points[1], m_points[0]);

			return circleComponent;
		}
	}
}
