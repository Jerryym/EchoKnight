using UnityEditor;
using UnityEngine;

namespace Echo.Editor
{
	public static class PickController
	{
		private const float PICK_THRESHOLD = 10f;

		[InitializeOnLoadMethod]
		private static void Init()
		{
			// 注册全局 SceneView GUI 回调
			SceneView.duringSceneGui += OnSceneGUI;
		}

		private static void OnSceneGUI(SceneView view)
		{
			Event e = Event.current;
			if (e.type != EventType.MouseDown || e.button != 0)
				return;

			Vector2 mousePos = e.mousePosition;
			GameObject pickedGO = PickSelectable(mousePos);
			if (pickedGO != null)
			{
				Selection.activeGameObject = pickedGO;
				e.Use();
			}
		}

		private static GameObject PickSelectable(Vector2 mousePos)
		{
			var selections = PickManager.Pickables;

			float minDist = float.MaxValue;
			IPickable selectionGO = null;
			foreach (var selection in selections)
			{
				if (selection.go == null)
					continue;

				float dist = selection.sel.HitObject(mousePos);
				if (dist < minDist)
				{
					minDist = dist;
					selectionGO = selection.sel;
				}
			}

			if (selectionGO != null && minDist < PICK_THRESHOLD)
				return selectionGO.GetGameObject();

			return null;
		}
	}
}
