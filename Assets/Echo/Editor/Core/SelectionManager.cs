using Echo.Component;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;

namespace Echo.Editor
{
	public static class SelectionManager
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
			var selections = SelectionRegistry.Selections;

			float minDist = float.MaxValue;
			ISelection selectionGO = null;
			foreach (var selection in selections)
			{
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

		/// <summary>
		/// 获取根据鼠标位置选中带指定组件的GameObject
		/// </summary>
		/// <typeparam name="T">目标组件类型</typeparam>
		/// <param name="mousePosition">Event.current.mousePosition</param>
		/// <returns>被选中的GameObject，如果没有选中返回 null</returns>
		private static GameObject PickGameObjectWithComponent<T>(Vector2 mousePosition) where T : UnityEngine.Component
		{
			GameObject pickedGO = HandleUtility.PickGameObject(mousePosition, false);
			if (pickedGO == null)
				return null;

			if (pickedGO.GetComponent<T>() != null)
				return pickedGO;

			return null;
		}
	}
}
