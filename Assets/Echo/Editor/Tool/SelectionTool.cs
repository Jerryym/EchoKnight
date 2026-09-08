using Echo.Editor.UI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor.Tool
{
	/// <summary>
	/// 选择集工具
	/// </summary>
	public class SelectionTool : IEditorTool
	{
		/// <summary>
		/// 提示词
		/// </summary>
		private string m_prompt;
		/// <summary>
		/// 是否选择预制体根节点
		/// </summary>
		private bool m_selectPrefabRoot = false;

		/// <summary>
		/// 完成回调
		/// </summary>
		private Action<IReadOnlyList<GameObject>> m_onComplete = null;

		/// <summary>
		/// 目标类型
		/// </summary>
		private Type m_targetType = null;

		/// <summary>
		/// 当前工具状态
		/// </summary>
		private ToolState m_state = ToolState.Idle;

		/// <summary>
		/// 选中的物体列表
		/// </summary>
		private List<GameObject> m_selectedObjs = new List<GameObject>();
		public IReadOnlyList<GameObject> SelectedObjs => m_selectedObjs;

		public SelectionTool(string prompt, bool selPrefabRoot, Action<IReadOnlyList<GameObject>> onComplete, Type targetType = null)
		{
			m_prompt = prompt;
			m_selectPrefabRoot = selPrefabRoot;
			m_onComplete = onComplete;
			m_targetType = targetType;
		}

		void IEditorTool.Activate()
		{
			m_state = ToolState.Running;
		}

		void IEditorTool.DeActivate()
		{
			m_state = ToolState.Idle;
			m_selectedObjs.Clear();
		}

		void IEditorTool.OnSceneGUI(SceneView sceneView)
		{
			if (m_state != ToolState.Running)
				return;

			//显示拾取光标
			PickCursor.Enabled = true;

			//显示提示词
			ShowPrompt();

			//事件过滤
			HandleInput();
		}

		/// <summary>
		/// 显示提示词
		/// </summary>
		private void ShowPrompt()
		{
			SceneView sceneView = RPGEditorToolWindow.ActiveWindow.SceneView;
			string text = $"{m_prompt} | 已选择: {m_selectedObjs.Count}\n按下空格键完成";

			Handles.BeginGUI();

			const float padding = 10f;
			GUIStyle style = new GUIStyle(GUI.skin.box)
			{
				alignment = TextAnchor.MiddleLeft,
				padding = new RectOffset(8, 8, 4, 4)
			};

			GUIContent content = new GUIContent(text);
			Vector2 size = style.CalcSize(content);

			Rect viewRect = sceneView.camera.pixelRect;
			Rect rect = new Rect(viewRect.x + padding, viewRect.yMax - size.y - padding, size.x, size.y);
			GUI.Box(rect, content, style);

			Handles.EndGUI();
		}

		private void HandleInput()
		{
			Event e = Event.current;
			switch (e.type)
			{
				case EventType.MouseDown://鼠标按下
					{
						if (e.button == 0)//左键
						{
							if (TryPick(e.mousePosition))
								e.Use();
						}
						else if (e.button == 1)//右键
						{
							Cancel();
							e.Use();
						}
						break;
					}
				case EventType.KeyDown:
					{
						if (e.keyCode == KeyCode.Escape)
						{
							Cancel();
							e.Use();
						}
						else if (e.keyCode == KeyCode.Space)
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

		private bool TryPick(Vector2 mousePt)
		{
			GameObject targeGO = PickController.Pick(mousePt);
			if (targeGO == null)
				return false;

			if (m_selectedObjs.Contains(targeGO))
				return false;

			//目标类型过滤
			if (m_targetType != null)
			{
				bool hasTarget = targeGO.GetComponent(m_targetType) != null;
				if (!hasTarget)
					return false;
			}

			Debug.Log($"selected objetName: {targeGO.name}");
			m_selectedObjs.Add(targeGO);

			//高亮选中的对象
			Selection.objects = m_selectedObjs.ToArray();
			return true;
		}

		/// <summary>
		/// 确定
		/// </summary>
		private void Finish()
		{
			m_state = ToolState.Completed;
			m_onComplete?.Invoke(m_selectedObjs);
			EditorToolManager.ClearTool();

			//取消高亮显示
			Selection.objects = null;

			//取消拾取光标显示
			PickCursor.Enabled = false;
			SceneView.RepaintAll();
		}

		/// <summary>
		/// 取消
		/// </summary>
		private void Cancel()
		{
			m_state = ToolState.Cancelled;
			EditorToolManager.ClearTool();

			//取消高亮显示
			Selection.objects = null;

			//取消拾取光标显示
			PickCursor.Enabled = false;
			SceneView.RepaintAll();
		}
	}
}
