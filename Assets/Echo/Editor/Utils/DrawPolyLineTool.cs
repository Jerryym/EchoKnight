using Echo.Component;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor.Utils
{
	/// <summary>
	/// 多段线绘制工具
	/// </summary>
	public class DrawPolyLineTool
	{
		/// <summary>
		/// 当前工具状态
		/// </summary>
		private DrawState m_currentState = DrawState.Idle;

		/// <summary>
		/// 多段线点数组
		/// </summary>
		private List<Vector3> m_points = null;

		private Vector3 m_lastPt = Vector3.zero;

		/// <summary>
		/// 当前活动SceneView
		/// </summary>
		private SceneView m_sceneView = null;

		public DrawPolyLineTool()
		{
			m_points = new List<Vector3>();
			m_sceneView = RPGEditorToolWindow.ActiveWindow.SceneView;
		}

		public void Activate()
		{
			SceneView.duringSceneGui += OnSceneGUI;
			m_currentState = DrawState.Drawing;
		}

		public void DeActivate()
		{
			SceneView.duringSceneGui -= OnSceneGUI;
			ResetTool();
		}

		private void OnSceneGUI(SceneView view)
		{
			Event e = Event.current;
			HandleInput(e);
			DrawPreview();
			m_sceneView.Repaint();
		}

		private void HandleInput(Event e)
		{
			if (m_currentState != DrawState.Drawing)
				return;

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
							//显示菜单栏
							ShowContextMenu();
							e.Use();
						}
						break;
					}
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

		private void AddPoint()
		{
			m_points.Add(m_lastPt);
		}

		private void ShowContextMenu()
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("确定"), false, Finish);
			menu.AddItem(new GUIContent("取消"), false, Cancel);
			menu.ShowAsContext();
		}

		private void DrawPreview()
		{
			if (m_points.Count == 0)
				return;

			Handles.color = Color.white;
			Handles.DrawAAPolyLine(1f, m_points.ToArray());

			//预览线
			Handles.DrawLine(m_points[m_points.Count - 1], m_lastPt, 1f);
		}

		private void UpdatePreview(Vector2 position)
		{
			Ray ray = HandleUtility.GUIPointToWorldRay(position);
			Plane plane = new Plane(Vector3.up, Vector3.zero);
			if (plane.Raycast(ray, out float enter))
			{
				m_lastPt = ray.GetPoint(enter);
			}
		}

		/// <summary>
		/// 确定
		/// </summary>
		private void Finish()
		{
			Debug.Log("DrawPolyLineTool: Finish");
			if (m_points.Count < 2)
			{
				ResetTool();
				return;
			}

			if (!EditorTool.TagExist("Polyline"))
				EditorTool.AddTag("Polyline");

			GameObject polyLineGO = new GameObject("多段线");
			polyLineGO.tag = "Polyline";
			Undo.RegisterCreatedObjectUndo(polyLineGO, "Draw PolyLine");

			Vector3 center = GetPolyLineCenterPt();
			polyLineGO.transform.position = center;

			//创建多段线组件
			var polyline = polyLineGO.AddComponent<PolyLine>();
			foreach (var pt in m_points)
			{
				polyline.AddPoint(pt - center);
			}

			//创建LineRender组件
			var lineRender = polyLineGO.AddComponent<LineRenderer>();
			lineRender.positionCount = m_points.Count;
			lineRender.SetPositions(m_points.ToArray());
			lineRender.widthCurve = AnimationCurve.Constant(0, 1f, 0.025f);
			lineRender.material = new Material(Shader.Find("Unlit/Color")) { color = polyline.Color };

			DeActivate();
		}

		/// <summary>
		/// 取消
		/// </summary>
		private void Cancel()
		{
			Debug.Log("DrawPolyLineTool: Cancel");
			DeActivate();
		}

		/// <summary>
		/// 重置工具
		/// </summary>
		private void ResetTool()
		{
			m_points.Clear();
			m_currentState = DrawState.Idle;
		}

		private Vector3 GetPolyLineCenterPt()
		{
			Vector3 sum = Vector3.zero;
			foreach (var pt in m_points)
			{
				sum += pt;
			}
			return sum / m_points.Count;
		}
	}
}
