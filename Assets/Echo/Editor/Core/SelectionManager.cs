using Echo.Component;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor
{
	public static class SelectionManager
	{
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

			Vector2 mousePos = Event.current.mousePosition;
			mousePos.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePos.y;
			GameObject pickedGO = PickGameObjectWithComponent<PolyLine>(mousePos);
			if (pickedGO != null)
			{
				Selection.activeGameObject = pickedGO;
				e.Use();
				Debug.Log("选中了 PolyLine: " + pickedGO.name);
			}
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
