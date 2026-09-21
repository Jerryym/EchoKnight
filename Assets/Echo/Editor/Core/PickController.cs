using Echo.Component;
using System.Collections.Generic;
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
			var selections = PickManager.Pickables;

			float minDist = float.MaxValue;
			IPickable selectionGO = null;
			foreach (var selection in selections)
			{
				if (selection.go == null)
					continue;

				float dist = GetScreenDistance(selection.sel);
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

		private static float GetScreenDistance(IPickable pickable)
		{
			if (pickable is not Curve curve)
				return float.MaxValue;

			List<Vector3> points = curve.GetWorldPoints();
			if (curve is PolyLine polyLine && polyLine.Closed && points.Count > 0)
			{
				points.Add(points[0]);
			}

			if (points.Count < 2)
				return float.MaxValue;

			return HandleUtility.DistanceToPolyLine(points.ToArray());
		}
	}
}
