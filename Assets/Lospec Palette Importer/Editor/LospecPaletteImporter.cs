using YesDev.Lospec;
using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace YesDev.Lospec
{
	public class LospecPaletteImporter : EditorWindow
	{
		string directoryPath = "Assets\\Lospec\\Palettes";
		string URL = "https://lospec.com/palette-list/byte-12";
		bool CreateColorPalette = true;
		public bool CreateScriptableObject = true;

		[MenuItem("Tools/ Lospec Palette")]
		public static void ShowWindow()
		{
			GetWindow(typeof(LospecPaletteImporter));
		}
		private void OnGUI()
		{
			GUILayout.Label("Lospec Color Palette Importer", EditorStyles.boldLabel);
			URL = EditorGUILayout.TextField("URL",URL);
			CreateColorPalette = EditorGUILayout.Toggle("Create Color Palette",CreateColorPalette);
			CreateScriptableObject = EditorGUILayout.Toggle("Create Scriptable Object",CreateScriptableObject);
			if (GUILayout.Button("Create Palette"))
			{
				Execute();
			}
		}

		public void Execute()
		{
			if (!CreateColorPalette && !CreateScriptableObject) return;
			if (URL == null || URL.Length == 0) return;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + ".json");
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			StreamReader reader = new StreamReader(response.GetResponseStream());
			string stringJson = reader.ReadToEnd();

			if (stringJson.Contains("error")) return;

			PaletteInformation paletteInformation = JsonUtility.FromJson<PaletteInformation>(stringJson);
			if (CreateColorPalette)
			{
				GenerateColorPalette(paletteInformation);
			}
			if (CreateScriptableObject)
			{
				GenerateScriptableObject(paletteInformation);
			}

		}

		private void GenerateColorPalette(PaletteInformation paletteInformation)
		{
			string Path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Unity\\Editor-5.x\\Preferences\\Presets";

			Path += $"/{paletteInformation.name}.colors";

			StringBuilder newString = new StringBuilder();
			newString.Append("%YAML 1.1");
			newString.AppendLine("%TAG !u! tag:unity3d.com,2011:");
			newString.AppendLine("--- !u!114 &1");
			newString.AppendLine("MonoBehaviour:");
			newString.AppendLine("  m_ObjectHideFlags: 52");
			newString.AppendLine("  m_CorrespondingSourceObject: {fileID: 0}");
			newString.AppendLine("  m_PrefabInstance: {fileID: 0}");
			newString.AppendLine("  m_PrefabAsset: {fileID: 0}");
			newString.AppendLine("  m_GameObject: {fileID: 0}");
			newString.AppendLine("  m_Enabled: 1");
			newString.AppendLine("  m_EditorHideFlags: 0");
			newString.AppendLine("  m_Script: {fileID: 12323, guid: 0000000000000000e000000000000000, type: 0}");
			newString.AppendLine("  m_Name: ");
			newString.AppendLine("  m_EditorClassIdentifier: ");
			newString.AppendLine("  m_Presets:");

			foreach (Color color in paletteInformation.GetColors())
			{
				newString.AppendLine("  - m_Name:");
				newString.AppendLine("    m_Color: {r: " + color.r + ", g: " + color.g + ", b: " + color.b + ", a: 1}");
			}

			File.WriteAllText(Path, newString.ToString());
			Debug.Log($"Sucessfully added {paletteInformation.name}");
		}
		private void GenerateScriptableObject(PaletteInformation paletteInformation)
		{
			string path = $"{directoryPath}\\{paletteInformation.name}.asset";
			LospecPaletteObject asset = ScriptableObject.CreateInstance<LospecPaletteObject>();
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);

			}
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();

			asset.SetUp(paletteInformation);
			Debug.Log($"Sucessfully added {paletteInformation.name}, scriptableObject");
		}
		public string PageUrl(int index)
		{
			return "https://lospec.com/palette-list/load?colorNumberFilterType=any&colorNumber=8&page=" + index + "&tag=&sortingType=default";
		}

		public string PaletteUrl(string pid)
		{
			return "https://lospec.com/palette-list/" + pid;
		}

	}
}

