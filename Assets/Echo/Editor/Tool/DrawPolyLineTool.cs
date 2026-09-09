using System.Collections.Generic;
using Echo.Component;
using Echo.Editor.Utils;
using Echo.Utils;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor.Tool
{
	/// <summary>
	/// 多段线绘制工具
	/// </summary>
	public class DrawPolyLineTool : IEditorTool
	{
		/// <summary>
		/// 当前工具状态
		/// </summary>
		private ToolState m_state = ToolState.Idle;

		/// <summary>
		/// 多段线点数组
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

		private const string m_tag = "Polyline";

		public DrawPolyLineTool()
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
			m_points.Add(m_previewPt);
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
			Handles.DrawLine(m_points[m_points.Count - 1], m_previewPt, 1f);

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

		/// <summary>
		/// 确定
		/// </summary>
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

			GameObject polyLineGO = new GameObject("多段线");
			polyLineGO.tag = m_tag;
			Undo.RegisterCreatedObjectUndo(polyLineGO, "Draw PolyLine");

			//创建多段线组件
			var polyline = CreatePolyLine(polyLineGO);
			//注册到选择集中
			PickManager.Register(polyLineGO, polyline);

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

		/// <summary>
		/// 创建多段线组件
		/// </summary>
		private PolyLine CreatePolyLine(GameObject polyLineGO)
		{
			//创建多段线组件
			var polyline = polyLineGO.AddComponent<PolyLine>();
			
			//判断是否闭合
			bool isClosed = GeometryTool.IsPolyLineClosed(m_points);
			if (isClosed)
				m_points.RemoveAt(m_points.Count - 1);
			
			//转为局部坐标
			Vector3 startPt = m_points[0];
			polyLineGO.transform.position = startPt;
			foreach (var pt in m_points)
			{
				polyline.AddPoint(pt - startPt);
			}
			
			//设置闭合状态
			polyline.SetClosed(isClosed);
			return polyline;
		}
	}
}
