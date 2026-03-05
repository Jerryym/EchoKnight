using UnityEditor;

namespace Echo.Editor.Utils
{
	/// <summary>
	/// 编辑器工具
	/// </summary>
	public static class EditorTool
	{
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
	}
}
