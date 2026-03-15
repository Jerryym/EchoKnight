using BehaviorDesigner.Runtime.Formations.Tasks;
using Echo.Component;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor.Tool
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

		private const string m_tag = "Polyline";

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
			if (m_points.Count < 2)
			{
				DeActivate();
				return;
			}

			if (!Utils.EditorTool.TagExist(m_tag))
				Utils.EditorTool.AddTag(m_tag);

			GameObject polyLineGO = new GameObject("多段线");
			polyLineGO.tag = m_tag;
			Undo.RegisterCreatedObjectUndo(polyLineGO, "Draw PolyLine");

			
			//创建多段线组件
			var polyline = CreatePolyLine(polyLineGO);
			//注册到选择集中
			SelectionManager.Register(polyLineGO, polyline);

			DeActivate();
		}

		/// <summary>
		/// 取消
		/// </summary>
		private void Cancel()
		{
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

		/// <summary>
		/// 创建多段线组件
		/// </summary>
		private PolyLine CreatePolyLine(GameObject polyLineGO)
		{
			//创建多段线组件
			var polyline = polyLineGO.AddComponent<PolyLine>();
			
			//转为局部坐标
			Vector3 startPt = m_points[0];
			polyLineGO.transform.position = startPt;
			foreach (var pt in m_points)
			{
				polyline.AddPoint(pt - startPt);
			}

			//判断是否闭合
			if (Utils.EditorTool.IsPolyLineClosed(m_points))
			{
				m_points.RemoveAt(m_points.Count - 1);
				polyline.SetClosed(true);
			}

			return polyline;
		}
	}
}
