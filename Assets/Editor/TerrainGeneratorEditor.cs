using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public class TerrainGeneratorEditor : EditorWindow
{
	private enum TerrainEnum
	{
		Plain,		//平原
		Mountain,	//山地
	}

	private	TerrainEnum m_TerrainEnum = TerrainEnum.Plain;
	private TerrainSetting m_TerrainSetting = null;
	private const string CONFIG_FOLDER_PATH = "Assets/Configs";

	[MenuItem("Window/PTG Editor")]
	static void Init()
	{
		TerrainGeneratorEditor editor = (TerrainGeneratorEditor)GetWindow(typeof(TerrainGeneratorEditor), false, "Procedural Terrain Generation");
		editor.Show();
	}

	private void OnGUI()
	{
		//地形配置文件
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("地形设置文件", EditorStyles.boldLabel);
		m_TerrainSetting = (TerrainSetting)EditorGUILayout.ObjectField("Terrain Setting", m_TerrainSetting, typeof(TerrainSetting), false);
		if (m_TerrainSetting == null)
		{
			EditorGUILayout.HelpBox("请先指定一个 TerrainSetting 资源。", MessageType.Warning);
			return;
		}

		//地形预设
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("地形预设", EditorStyles.boldLabel);
		TerrainEnum newPreset = (TerrainEnum)EditorGUILayout.EnumPopup("预设类型", m_TerrainEnum);
		if (newPreset != m_TerrainEnum)
		{
			m_TerrainEnum = newPreset;
			ApplyPresetToSetting();
		}

		//参数
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("地形参数", EditorStyles.boldLabel);
		EditorGUILayout.BeginVertical("box");

		m_TerrainSetting.terrainWidth = EditorGUILayout.IntField("地形宽度 (X)", m_TerrainSetting.terrainWidth);
		m_TerrainSetting.terrainLength = EditorGUILayout.IntField("地形长度 (Z)", m_TerrainSetting.terrainLength);
		m_TerrainSetting.chunkSize = EditorGUILayout.IntField("分块大小", m_TerrainSetting.chunkSize);

		m_TerrainSetting.maxHeight = EditorGUILayout.FloatField("地形最大高度", m_TerrainSetting.maxHeight);
		m_TerrainSetting.heightCurve = EditorGUILayout.CurveField("高度曲线", m_TerrainSetting.heightCurve, Color.green, new Rect(0, 0, 1, 1));

		m_TerrainSetting.noiseScale = EditorGUILayout.FloatField("噪声缩放", m_TerrainSetting.noiseScale);
		m_TerrainSetting.octaves = EditorGUILayout.IntSlider("噪声层数", m_TerrainSetting.octaves, 1, 8);
		m_TerrainSetting.persistence = EditorGUILayout.Slider("持久度 (Persistence)", m_TerrainSetting.persistence, 0f, 1f);
		m_TerrainSetting.lacunarity = EditorGUILayout.Slider("空隙度 (Lacunarity)", m_TerrainSetting.lacunarity, 1f, 4f);

		EditorGUILayout.EndVertical();

		//=====================================按钮=====================================
		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();

		//保存设置按钮
		if (GUILayout.Button("保存设置"))
		{
			SaveTerrainSetting();
		}

		//更新设置按钮
		GUI.enabled = m_TerrainSetting != null;
		if (GUILayout.Button("更新设置"))
		{
			UpdateTerrainSetting();
		}
		GUI.enabled = true;

		//生成按钮
		if (GUILayout.Button("生成"))
		{
			TerrainGenerator.Generate(m_TerrainSetting);
		}
		EditorGUILayout.EndHorizontal();
	}

	private void OnEnable()
	{
		if (m_TerrainSetting == null)
		{
			m_TerrainSetting = ScriptableObject.CreateInstance<TerrainSetting>();
			m_TerrainSetting.name = "Unsaved Terrain Setting";
		}
	}

	private void ApplyPresetToSetting()
	{
		switch (m_TerrainEnum)
		{
			case TerrainEnum.Plain:
				m_TerrainSetting.noiseScale = 100f;
				m_TerrainSetting.maxHeight = 10f;
				m_TerrainSetting.octaves = 2;
				m_TerrainSetting.persistence = 0.4f;
				m_TerrainSetting.lacunarity = 1.8f;
				m_TerrainSetting.heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
				break;

			case TerrainEnum.Mountain:
				m_TerrainSetting.noiseScale = 40f;
				m_TerrainSetting.maxHeight = 50f;
				m_TerrainSetting.octaves = 5;
				m_TerrainSetting.persistence = 0.6f;
				m_TerrainSetting.lacunarity = 2.2f;
				m_TerrainSetting.heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
				break;
		}

		EditorUtility.SetDirty(m_TerrainSetting);
		Repaint();
	}

	private void SaveTerrainSetting()
	{
		// 确保路径存在
		if (!Directory.Exists(CONFIG_FOLDER_PATH))
			Directory.CreateDirectory(CONFIG_FOLDER_PATH);

		// 打开保存对话框
		string path = EditorUtility.SaveFilePanelInProject(
			"保存地形配置",
			"NewTerrainSetting",
			"asset",
			"请选择保存位置",
			CONFIG_FOLDER_PATH
		);

		if (string.IsNullOrEmpty(path))
			return;

		//创建新的SO实例
		TerrainSetting setting = ScriptableObject.CreateInstance<TerrainSetting>();
		EditorUtility.SetDirty(m_TerrainSetting);

		AssetDatabase.CreateAsset(setting, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		m_TerrainSetting = setting; // 自动指向新SO
	}

	private void UpdateTerrainSetting()
	{
		if (m_TerrainSetting == null)
			return;

		EditorUtility.SetDirty(m_TerrainSetting);
		AssetDatabase.SaveAssets();
	}
}
