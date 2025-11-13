using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TerrainGeneratorEditor : EditorWindow
{
	private enum TerrainEnum
	{
		Plain,		//平原
		Hill,		//丘陵
		Mountain,	//山地
	}

	private TerrainSetting m_currentSetting = null;
	private Material m_TerrainHeighMat = null;
	private	TerrainEnum m_TerrainEnum = TerrainEnum.Plain;
	private const string CONFIG_FOLDER_PATH = "Assets/Configs";
	private bool m_HasLoadedSetting = false;

	#region 地形尺寸
	/// <summary>
	/// 地形横向宽度(X)
	/// </summary>
	[Min(1)] private int m_terrainWidth = 256;
	/// <summary>
	/// 地形纵向宽度(Z)
	/// </summary>
	[Min(1)] public int m_terrainLength = 256;
	/// <summary>
	/// 分块大小
	/// </summary>
	[Min(1)] public int m_chunkSize = 32;
	#endregion

	#region 高度设置
	/// <summary>
	/// 地形最小高度
	/// </summary>
	[Range(1f, 200f)] public float m_minHeight = 0.0f;
	/// <summary>
	/// 地形最大高度
	/// </summary>
	[Range(1f, 200f)] public float m_maxHeight = 20f;
	/// <summary>
	/// 高度曲线
	/// </summary>
	public AnimationCurve m_heightCurve = new AnimationCurve(
		new Keyframe(0, 0),
		new Keyframe(0.3f, 0.1f),
		new Keyframe(0.6f, 0.8f),
		new Keyframe(1f, 1f)
	);
	#endregion

	#region 噪声参数
	/// <summary>
	/// 噪声缩放
	/// </summary>
	[Range(1f, 500f)] public float m_noiseScale = 50.0f;
	/// <summary>
	/// 噪声层数
	/// </summary>
	[Range(1, 8)] public int m_octaves = 4;
	/// <summary>
	/// 持久度
	/// </summary>
	[Range(0f, 1f)] public float m_persistence = 0.5f;
	/// <summary>
	/// 空隙度
	/// </summary>
	[Range(1f, 4f)] public float m_lacunarity = 2.0f;
	#endregion

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
		TerrainSetting newSetting = (TerrainSetting)EditorGUILayout.ObjectField("Terrain Setting", m_currentSetting, typeof(TerrainSetting), false);
		if (newSetting != m_currentSetting)
		{
			m_currentSetting = newSetting;
			m_HasLoadedSetting = false; // 重新加载一次数据
			Repaint();
		}

		if (m_currentSetting == null)
		{
			EditorGUILayout.HelpBox("请先指定一个 TerrainSetting 资源。", MessageType.Warning);
		}

		if (!m_HasLoadedSetting)
		{
			SetSettingData();
			m_HasLoadedSetting = true;
		}

		//材质
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("地形材质", EditorStyles.boldLabel);
		m_TerrainHeighMat = (Material)EditorGUILayout.ObjectField("Terrain Material", m_TerrainHeighMat, typeof(Material), false);
		if (m_TerrainHeighMat == null)
		{
			EditorGUILayout.HelpBox("请先指定一个 Terrain Material。", MessageType.Warning);
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

		//地形参数
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("地形参数", EditorStyles.boldLabel);
		EditorGUILayout.BeginVertical("box");
		m_terrainWidth = EditorGUILayout.IntField("地形宽度 (X)", m_terrainWidth);
		m_terrainLength = EditorGUILayout.IntField("地形长度 (Z)", m_terrainLength);
		m_chunkSize = EditorGUILayout.IntField("分块大小", m_chunkSize);
		m_minHeight = EditorGUILayout.FloatField("地形最小高度", m_minHeight);
		m_maxHeight = EditorGUILayout.FloatField("地形最大高度", m_maxHeight);
		m_heightCurve = EditorGUILayout.CurveField("高度曲线", m_heightCurve, Color.green, new Rect(0, 0, 1, 1));
		EditorGUILayout.EndVertical();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("噪声配置参数", EditorStyles.boldLabel);
		EditorGUILayout.BeginVertical("box");
		m_noiseScale = EditorGUILayout.FloatField("噪声缩放", m_noiseScale);
		m_octaves = EditorGUILayout.IntSlider("噪声层数", m_octaves, 1, 8);
		m_persistence = EditorGUILayout.Slider("持久度 (Persistence)", m_persistence, 0f, 1f);
		m_lacunarity = EditorGUILayout.Slider("空隙度 (Lacunarity)", m_lacunarity, 1f, 4f);
		EditorGUILayout.EndVertical();

		#region 按钮
		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();

		//保存设置按钮
		if (GUILayout.Button("保存设置"))
		{
			SaveTerrainSetting();
		}

		//更新设置按钮
		GUI.enabled = (m_currentSetting != null && m_TerrainHeighMat != null);
		if (GUILayout.Button("更新设置"))
		{
			UpdateTerrainSetting();
		}
		GUI.enabled = true;

		//生成按钮
		if (GUILayout.Button("生成"))
		{
			GetSettingData(m_currentSetting);
			TerrainGenerator.Generate(m_currentSetting, m_TerrainHeighMat);
		}
		EditorGUILayout.EndHorizontal();
		#endregion
	}

	private void OnEnable()
	{
		if (m_currentSetting == null)
		{
			m_currentSetting = ScriptableObject.CreateInstance<TerrainSetting>();
			m_currentSetting.name = "Unsaved Terrain Setting";
		}
	}

	private void ApplyPresetToSetting()
	{
		switch (m_TerrainEnum)
		{
			case TerrainEnum.Plain:
				m_noiseScale = 100f;
				m_minHeight = 10f;
				m_maxHeight = 60f;
				m_octaves = 2;
				m_persistence = 0.4f;
				m_lacunarity = 1.8f;
				m_heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
				break;
			case TerrainEnum.Hill:
				m_noiseScale = 60f;
				m_minHeight = 60f;
				m_maxHeight = 100f;
				m_octaves = 4;
				m_persistence = 0.5f;
				m_lacunarity = 2.2f;
				m_heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
				break;
			case TerrainEnum.Mountain:
				m_noiseScale = 40f;
				m_minHeight = 100f;
				m_maxHeight = 150f;
				m_octaves = 5;
				m_persistence = 0.6f;
				m_lacunarity = 2.2f;
				m_heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
				break;
		}
		Repaint();
	}

	private void SaveTerrainSetting()
	{
		if (!Directory.Exists(CONFIG_FOLDER_PATH))
			Directory.CreateDirectory(CONFIG_FOLDER_PATH);

		//打开保存对话框
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
		GetSettingData(setting);

		AssetDatabase.CreateAsset(setting, path);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		m_currentSetting = setting;
	}

	private void UpdateTerrainSetting()
	{
		if (m_currentSetting == null)
			return;

		GetSettingData(m_currentSetting);
		EditorUtility.SetDirty(m_currentSetting);
		AssetDatabase.SaveAssets();
	}

	private void SetSettingData()
	{
		m_terrainWidth = m_currentSetting.terrainWidth;
		m_terrainLength = m_currentSetting.terrainLength;
		m_chunkSize = m_currentSetting.chunkSize;

		m_minHeight = m_currentSetting.minHeight;
		m_maxHeight = m_currentSetting.maxHeight;
		m_heightCurve = m_currentSetting.heightCurve;

		m_noiseScale = m_currentSetting.noiseScale;
		m_octaves = m_currentSetting.octaves;
		m_persistence = m_currentSetting.persistence;
		m_lacunarity = m_currentSetting.lacunarity;
	}

	private void GetSettingData(TerrainSetting setting)
	{
		setting.terrainWidth = m_terrainWidth;
		setting.terrainLength = m_terrainLength;
		setting.chunkSize = m_chunkSize;

		setting.minHeight = m_minHeight;
		setting.maxHeight = m_maxHeight;
		setting.heightCurve = m_heightCurve;

		setting.noiseScale = m_noiseScale;
		setting.octaves = m_octaves;
		setting.persistence = m_persistence;
		setting.lacunarity = m_lacunarity;
	}
}
