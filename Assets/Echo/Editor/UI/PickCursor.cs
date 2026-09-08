using System;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor.UI
{
	/// <summary>
	/// 拾取光标
	/// </summary>
	public static class PickCursor
	{
		public static bool Enabled { get; set; } = false;
		public static float Size { get; set; } = 12f;

		public static void Draw()
		{
			if (!Enabled)
				return;

			Event e = Event.current;
			Vector2 mouse = e.mousePosition;

			// 隐藏 Unity 默认鼠标
			EditorGUIUtility.AddCursorRect(new Rect(0, 0, Screen.width, Screen.height), MouseCursor.CustomCursor);

			//绘制拾取光标
			DrawPickBox(mouse);
		}

		private static void DrawPickBox(Vector2 mouse)
		{
			float half = Size * 0.5f;

			Vector3 p1 = new Vector3(mouse.x - half, mouse.y - half);
			Vector3 p2 = new Vector3(mouse.x + half, mouse.y - half);
			Vector3 p3 = new Vector3(mouse.x + half, mouse.y + half);
			Vector3 p4 = new Vector3(mouse.x - half, mouse.y + half);

			Handles.BeginGUI();

			Handles.DrawLine(p1, p2);
			Handles.DrawLine(p2, p3);
			Handles.DrawLine(p3, p4);
			Handles.DrawLine(p4, p1);

			Handles.EndGUI();
		}
	}
}
