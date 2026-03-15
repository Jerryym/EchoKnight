using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor.Utils
{
	/// <summary>
	/// 编辑器工具
	/// </summary>
	public static class EditorTool
	{
		#region Tag
		/// <summary>
		/// 判断Tag是否存在
		/// </summary>
		public static bool TagExist(string tagName)
		{
			if (string.IsNullOrEmpty(tagName))
				return false;

			var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
			if (asset == null || asset.Length == 0)
				return false;

			var tagManager = new SerializedObject(asset[0]);
			SerializedProperty tagsProp = tagManager.FindProperty("tags");

			//判断是否存在同名Tag
			bool isExist = false;
			for (int i = 0; i < tagsProp.arraySize; i++)
			{
				if (tagsProp.GetArrayElementAtIndex(i).stringValue == tagName)
				{
					isExist = true;
					break;
				}
			}
			return isExist;
		}

		/// <summary>
		/// 添加Tag
		/// </summary>
		public static void AddTag(string tagName)
		{
			if (string.IsNullOrEmpty(tagName))
				return;

			//判断是否存在同名Tag
			bool isExist = TagExist(tagName);

			//创建新Tag
			if (!isExist)
			{
				var asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
				if (asset == null || asset.Length == 0)
					return;

				var tagManager = new SerializedObject(asset[0]);
				SerializedProperty tagsProp = tagManager.FindProperty("tags");

				tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
				var newTag = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
				newTag.stringValue = tagName;
				tagManager.ApplyModifiedProperties();
			}
		}
		#endregion

		#region Math
		/// <summary>
		/// 获取最接近的点
		/// </summary>
		public static bool GetClosestPoint(List<Vector3> points, in Vector3 basePt, out Vector3 targetPt)
		{
			targetPt = Vector3.zero;
			float minDistance = float.MaxValue;
			bool isFind = false;

			foreach (var pt in points)
			{
				float distance = Vector3.Distance(pt, basePt);
				if (distance < minDistance)
				{
					minDistance = distance;
					targetPt = pt;
					isFind = true;
				}
			}
			return isFind;
		}

		/// <summary>
		/// 判断多段线是否闭合
		/// </summary>
		public static bool IsPolyLineClosed(List<Vector3> points)
		{
			if (points == null || points.Count < 3)
				return false;

			Vector3 firstPt = points[0];
			Vector3 lastPt = points[points.Count - 1];
			return Vector3.Distance(firstPt, lastPt) < 1e-3;
		}
		#endregion
	}
}
