using UnityEditor;
using UnityEngine;

namespace Echo.Editor
{
	/// <summary>
	/// 拾取控制器
	/// </summary>
	public static class PickController
	{
		private const float PICK_THRESHOLD = 10f;

		public static GameObject Pick(Vector2 mousePt, bool selPrefabRoot = true)
		{
			GameObject targetGO = null;

			//可拾取对象
			GameObject pickableGO = PickSelectable(mousePt);
			if (pickableGO != null)
			{
				targetGO = pickableGO;
			}

			//默认拾取
			GameObject unityPickGO = HandleUtility.PickGameObject(mousePt, selPrefabRoot);
			if (unityPickGO != null)
			{
				if (targetGO == null)
				{
					targetGO = unityPickGO;
				}
			}

			return targetGO;
		}

		private static GameObject PickSelectable(Vector2 mousePt)
		{
			//设置拾取平面
			Plane plane = new Plane(Vector3.up, Vector3.zero);
			Ray ray = HandleUtility.GUIPointToWorldRay(mousePt);
			if (!plane.Raycast(ray, out float enter))
				return null;

			Vector3 mouseWorld = ray.GetPoint(enter);

			var selections = PickManager.Pickables;

			float minDist = float.MaxValue;
			IPickable selectionGO = null;
			foreach (var selection in selections)
			{
				if (selection.go == null)
					continue;

				float dist = selection.sel.HitObject(mouseWorld);
				if (dist >= PICK_THRESHOLD)
					continue;

				if (dist < minDist)
				{
					minDist = dist;
					selectionGO = selection.sel;
				}
			}

			return selectionGO?.GetGameObject();
		}
	}
}
