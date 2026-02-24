using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor.UI
{
	public enum PathMode
	{
		OpenFolder,
		OpenFile,
		SaveFile,
	}

	public class PathField : VisualElement
	{
		private TextField m_textField = null;
		private Button m_browseBtn = null;
		
		private PathMode m_mode;
		/// <summary>
		/// 文件默认名
		/// </summary>
		private string m_defaultFileName;
		/// <summary>
		/// 文件扩展名
		/// </summary>
		private string m_extension;
		/// <summary>
		/// 是否使用相对于Assets的路径
		/// </summary>
		private bool m_useAssetPath = true;
		private string m_absolutePath;

		public string Path
		{
			get => m_textField.value;
			set => m_textField.value = value;
		}

		public PathField(string label, PathMode mode = PathMode.OpenFolder, string defaultFileName = "", string extension = "", bool useAssetPath = true)
		{
			m_mode = mode;
			m_defaultFileName = defaultFileName;
			m_extension = extension;
			m_useAssetPath = useAssetPath;

			style.flexDirection = FlexDirection.Row;
			style.width = Length.Percent(100);

			m_textField = new TextField(label);
			m_textField.style.flexGrow = 1;
			m_textField.style.marginRight = 4;
			Add(m_textField);

			m_browseBtn = new Button();
			m_browseBtn.text = "...";
			m_browseBtn.clicked += OnBrowseBtnClicked;
			m_browseBtn.style.flexShrink = 0;
			m_browseBtn.style.width = 28;
			Add(m_browseBtn);
		}

		private void OnBrowseBtnClicked()
		{
			string pathName = "";
			switch (m_mode)
			{
				case PathMode.OpenFolder:
					pathName = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
					break;
				case PathMode.OpenFile:
					pathName = EditorUtility.OpenFilePanel("选择文件", Application.dataPath, m_extension);
					break;
				case PathMode.SaveFile:
					pathName = EditorUtility.SaveFilePanel("保存文件", Application.dataPath, m_defaultFileName, m_extension);
					break;
				default:
					break;
			}

			if (!string.IsNullOrEmpty(pathName))
			{
				m_absolutePath = pathName;
				if (m_useAssetPath)
				{
					m_textField.value = ConvertToProjectRelative(pathName);
				}
				else
				{
					m_textField.value = pathName;
				}
			}
		}

		private string ConvertToProjectRelative(string absolutePath)
		{
			if (string.IsNullOrEmpty(absolutePath))
				return "";

			string dataPath = Application.dataPath.Replace("\\", "/");
			absolutePath = absolutePath.Replace("\\", "/");
			if (!absolutePath.StartsWith(dataPath))
			{
				return absolutePath;
			}
			return "Assets" + absolutePath.Substring(dataPath.Length);
		}
	}
}
