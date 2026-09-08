using Echo.Editor.UI;
using UnityEditor;

namespace Echo.Editor
{
	[InitializeOnLoad]
	public static class ScenePickCursorHook
	{
		static ScenePickCursorHook()
		{
			SceneView.duringSceneGui += OnSceneGUI;
		}

		static void OnSceneGUI(SceneView sceneView)
		{
			PickCursor.Draw();
		}
	}
}
