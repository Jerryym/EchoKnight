using UnityEditor;
using UnityEngine;

namespace Echo.Editor
{
	/// <summary>
	/// 编辑器工具管理器
	/// </summary>
	[InitializeOnLoad]
	public static class EditorToolManager
	{
		private static IEditorTool s_activeTool;

		static EditorToolManager()
		{
			SceneView.duringSceneGui += OnSceneGUI;
		}

		public static void SetTool(IEditorTool editorTool)
		{
			if (editorTool == null)
				return;

			s_activeTool?.DeActivate();
			s_activeTool = editorTool;
			s_activeTool?.Activate();
		}

		public static void ClearTool()
		{
			s_activeTool?.DeActivate();
			s_activeTool = null;
		}

		private static void OnSceneGUI(SceneView view)
		{
			if (s_activeTool != null)
			{
				s_activeTool?.OnSceneGUI(view);
			}
			else
			{
				//默认选择
				PickGameObject(view);
			}
		}

		private static void PickGameObject(SceneView view)
		{
			Event e = Event.current;
			if (e.type != EventType.MouseDown || e.button != 0)
				return;

			Vector2 mousePt = e.mousePosition;
			GameObject targetGO = PickController.Pick(mousePt);
			if (targetGO != null)
			{
				Selection.activeGameObject = targetGO;
				e.Use();
			}
		}
	}
}
