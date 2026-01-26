using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TerrainGeneratorEditor : EditorWindow
{
	private enum TerrainType
	{
		Plain,		//平原
		Hill,		//丘陵
		Mountain,	//山地
	}

	/// <summary>
	/// 地形名称
	/// </summary>
	public string terrainName = "Terrain";
	#region 地形尺寸
	/// <summary>
	/// 地形横向宽度(X)
	/// </summary>
	[Min(1)] public int terrainWidth = 256;
	/// <summary>
	/// 地形纵向宽度(Z)
	/// </summary>
	[Min(1)] public int terrainLength = 256;
	/// <summary>
	/// 分块大小
	/// </summary>
	[Min(1)] public int chunkSize = 32;
	#endregion

	#region 高度设置
	/// <summary>
	/// 地形最小高度
	/// </summary>
	[Range(1f, 200f)] public float minHeight = 0.0f;
	/// <summary>
	/// 地形最大高度
	/// </summary>
	[Range(1f, 200f)] public float maxHeight = 20f;
	/// <summary>
	/// 高度曲线
	/// </summary>
	public AnimationCurve heightCurve = new AnimationCurve(
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
	[Range(1f, 500f)] public float noiseScale = 50.0f;
	/// <summary>
	/// 噪声层数
	/// </summary>
	[Range(1, 8)] public int octaves = 4;
	/// <summary>
	/// 持久度
	/// </summary>
	[Range(0f, 1f)] public float persistence = 0.5f;
	/// <summary>
	/// 空隙度
	/// </summary>
	[Range(1f, 4f)] public float lacunarity = 2.0f;
	#endregion

	private TerrainSetting m_currentSetting = null;
	private TerrainType m_TerrainType = TerrainType.Plain;
	private Material m_TerrainHeighMat = null;

	private const string CONFIG_FOLDER_PATH = "Assets/Configs";
	private bool m_HasLoadedSetting = false;
	private GameObject m_TerrainGo = null;

	[MenuItem("Tools/PTG Editor")]
	static void Init()
	{
		TerrainGeneratorEditor editor = (TerrainGeneratorEditor)GetWindow(typeof(TerrainGeneratorEditor), false, "PTG Editor");
		editor.Show();
	}

	private void OnGUI()
	{
		//地形配置文件
		DrawTerrainSettingSection();
		//地形参数
		DrawParamSection();
		//材质
		DrawMaterialSection();
		//按钮
		DrawButtons();
	}

	private void OnEnable()
	{
		if (m_currentSetting == null)
		{
			m_currentSetting = ScriptableObject.CreateInstance<TerrainSetting>();
			m_currentSetting.name = "Unsaved Terrain Setting";
		}
	}

	private void DrawTerrainSettingSection()
	{
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
	}

	private void DrawParamSection()
	{
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("参数", EditorStyles.boldLabel);
		EditorGUILayout.BeginVertical("box");

		terrainName = EditorGUILayout.TextField("地形名称", terrainName);
		TerrainType newType = (TerrainType)EditorGUILayout.EnumPopup("类型", m_TerrainType);
		if (newType != m_TerrainType)
		{
			m_TerrainType = newType;
			GetSettingByType();
		}

		//地形参数
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("地形参数", EditorStyles.boldLabel);
		EditorGUILayout.BeginVertical("box");
		terrainWidth = EditorGUILayout.IntField("地形宽度 (X)", terrainWidth);
		terrainLength = EditorGUILayout.IntField("地形长度 (Z)", terrainLength);
		chunkSize = EditorGUILayout.IntField("分块大小", chunkSize);
		minHeight = EditorGUILayout.FloatField("地形最小高度", minHeight);
		maxHeight = EditorGUILayout.FloatField("地形最大高度", maxHeight);
		heightCurve = EditorGUILayout.CurveField("高度曲线", heightCurve, Color.green, new Rect(0, 0, 1, 1));
		EditorGUILayout.EndVertical();

		//噪声配置参数
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("噪声配置参数", EditorStyles.boldLabel);
		EditorGUILayout.BeginVertical("box");
		noiseScale = EditorGUILayout.FloatField("噪声缩放", noiseScale);
		octaves = EditorGUILayout.IntSlider("噪声层数", octaves, 1, 8);
		persistence = EditorGUILayout.Slider("持久度 (Persistence)", persistence, 0f, 1f);
		lacunarity = EditorGUILayout.Slider("空隙度 (Lacunarity)", lacunarity, 1f, 4f);
		EditorGUILayout.EndVertical();

		EditorGUILayout.EndVertical();
	}

	private void DrawMaterialSection()
	{
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("材质", EditorStyles.boldLabel);
		m_TerrainHeighMat = (Material)EditorGUILayout.ObjectField("地形材质", m_TerrainHeighMat, typeof(Material), false);
		if (m_TerrainHeighMat == null)
		{
			EditorGUILayout.HelpBox("请设置地形材质", MessageType.Error);
		}
	}

	private void DrawButtons()
	{
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
			if (m_TerrainGo != null)
			{
				Destroy(m_TerrainGo);
			}

			GetSettingData(m_currentSetting);
			m_TerrainGo = TerrainGenerator.Generate(m_currentSetting, m_TerrainHeighMat);
		}
		EditorGUILayout.EndHorizontal();
	}

	private void GetSettingByType()
	{
		switch (m_TerrainType)
		{
			case TerrainType.Plain:
				noiseScale = 100f;
				minHeight = 10f;
				maxHeight = 60f;
				octaves = 2;
				persistence = 0.4f;
				lacunarity = 1.8f;
				heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
				break;
			case TerrainType.Hill:
				noiseScale = 60f;
				minHeight = 60f;
				maxHeight = 100f;
				octaves = 4;
				persistence = 0.5f;
				lacunarity = 2.2f;
				heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
				break;
			case TerrainType.Mountain:
				noiseScale = 40f;
				minHeight = 100f;
				maxHeight = 150f;
				octaves = 5;
				persistence = 0.6f;
				lacunarity = 2.2f;
				heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
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
		terrainWidth = m_currentSetting.terrainWidth;
		terrainLength = m_currentSetting.terrainLength;
		chunkSize = m_currentSetting.chunkSize;

		minHeight = m_currentSetting.minHeight;
		maxHeight = m_currentSetting.maxHeight;
		heightCurve = m_currentSetting.heightCurve;

		noiseScale = m_currentSetting.noiseScale;
		octaves = m_currentSetting.octaves;
		persistence = m_currentSetting.persistence;
		lacunarity = m_currentSetting.lacunarity;
	}

	private void GetSettingData(TerrainSetting setting)
	{
		setting.terrainWidth = terrainWidth;
		setting.terrainLength = terrainLength;
		setting.chunkSize = chunkSize;

		setting.minHeight = minHeight;
		setting.maxHeight = maxHeight;
		setting.heightCurve = heightCurve;

		setting.noiseScale = noiseScale;
		setting.octaves = octaves;
		setting.persistence = persistence;
		setting.lacunarity = lacunarity;
	}
}
