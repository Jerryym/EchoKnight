using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PCGEditorTool : EditorWindow
{
	private enum TerrainType
	{
		Plain,		//平原
		Hill,		//丘陵
		Mountain,	//山地
	}

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

	private string m_sTerrainName = "Terrain";
	private TerrainSetting m_currentSetting = null;
	private TerrainType m_TerrainType = TerrainType.Plain;
	private Material m_TerrainHeighMat = null;
	private bool m_bOpenLOD = true;
	private readonly string[] m_LODOptions = new string[] { "生成1层LOD", "生成2层LOD", "生成3层LOD" };
	private int m_iLODIndex = 0;

	private bool m_bFlodOut_TerrainParam = false;
	private bool m_bFlodOut_NoiseParam = false;

	private const string CONFIG_FOLDER_PATH = "Assets/Configs";
	private const string PREFAB_FOLDER_PATH = "Assets/Prefabs";
	private bool m_bHasLoadedSetting = false;
	private GameObject m_TerrainGo = null;

	[MenuItem("Tools/PCG Editor Tool")]
	static void Init()
	{
		PCGEditorTool editorTool = (PCGEditorTool)GetWindow(typeof(PCGEditorTool), false, "PCG Editor Tool");
		editorTool.Show();
	}

	private void OnGUI()
	{
		//地形配置文件
		DrawTerrainSetting();
		//地形参数
		DrawParam();
		//LOD
		DrawLODSetting();
		//按钮
		DrawButtons();
	}

	private void OnEnable()
	{
		m_currentSetting = null;
		m_bHasLoadedSetting = false;
	}

	private void DrawTerrainSetting()
	{
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("地形设置文件", EditorStyles.boldLabel);
		TerrainSetting newSetting = (TerrainSetting)EditorGUILayout.ObjectField("Terrain Setting", m_currentSetting, typeof(TerrainSetting), false);
		if (newSetting != m_currentSetting)
		{
			m_currentSetting = newSetting;
			m_bHasLoadedSetting = false; // 重新加载一次数据
			m_sTerrainName = (newSetting != null) ? newSetting.name : "";
			Repaint();
		}

		if (m_currentSetting == null)
		{
			m_bHasLoadedSetting = false;
			return;
		}

		if (!m_bHasLoadedSetting)
		{
			SetSettingData();
			m_bHasLoadedSetting = true;
		}
	}

	private void DrawParam()
	{
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("参数", EditorStyles.boldLabel);
		EditorGUILayout.BeginVertical("box");

		//地形名称
		m_sTerrainName = EditorGUILayout.TextField("地形名称", m_sTerrainName);

		//地形类型
		TerrainType newType = (TerrainType)EditorGUILayout.EnumPopup("地形类型", m_TerrainType);
		if (newType != m_TerrainType)
		{
			m_TerrainType = newType;
			GetSettingByType();
			Repaint();
		}

		//地形材质
		m_TerrainHeighMat = (Material)EditorGUILayout.ObjectField("地形材质", m_TerrainHeighMat, typeof(Material), false);
		if (m_TerrainHeighMat == null)
		{
			EditorGUILayout.HelpBox("请设置地形材质", MessageType.Error);
		}

		//地形参数
		EditorGUILayout.Space();
		m_bFlodOut_TerrainParam = EditorGUILayout.Foldout(m_bFlodOut_TerrainParam, "地形参数");
		if (m_bFlodOut_TerrainParam)
		{
			terrainWidth = EditorGUILayout.IntField("地形宽度 (X)", terrainWidth);
			terrainLength = EditorGUILayout.IntField("地形长度 (Z)", terrainLength);
			chunkSize = EditorGUILayout.IntField("分块大小", chunkSize);
			minHeight = EditorGUILayout.FloatField("地形最小高度", minHeight);
			maxHeight = EditorGUILayout.FloatField("地形最大高度", maxHeight);
			heightCurve = EditorGUILayout.CurveField("高度曲线", heightCurve, Color.green, new Rect(0, 0, 1, 1));
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		//噪声配置参数
		EditorGUILayout.Space();
		m_bFlodOut_NoiseParam = EditorGUILayout.Foldout(m_bFlodOut_NoiseParam, "噪声配置参数");
		if (m_bFlodOut_NoiseParam)
		{
			noiseScale = EditorGUILayout.FloatField("噪声缩放", noiseScale);
			octaves = EditorGUILayout.IntSlider("噪声层数", octaves, 1, 8);
			persistence = EditorGUILayout.Slider("持久度 (Persistence)", persistence, 0f, 1f);
			lacunarity = EditorGUILayout.Slider("空隙度 (Lacunarity)", lacunarity, 1f, 4f);
		}
		EditorGUILayout.EndFoldoutHeaderGroup();

		EditorGUILayout.EndVertical();
	}

	private void DrawLODSetting()
	{
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("LOD设置", EditorStyles.boldLabel);
		EditorGUILayout.BeginVertical("box");

		m_bOpenLOD = EditorGUILayout.Toggle("开启LOD", m_bOpenLOD);
		EditorGUI.BeginDisabledGroup(!m_bOpenLOD);
		m_iLODIndex = EditorGUILayout.Popup("LOD等级", m_iLODIndex, m_LODOptions);
		EditorGUI.EndDisabledGroup();		

		EditorGUILayout.EndVertical();
	}
	private void DrawButtons()
	{
		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();

		//保存/更新设置
		if (GUILayout.Button("保存/更新设置"))
		{
			SaveTerrainSetting();
		}

		//预览
		if (GUILayout.Button("预览"))
		{
			if (m_TerrainGo != null)
			{
				DestroyImmediate(m_TerrainGo);
			}
			m_currentSetting = GenerateTerrainSetting();
			m_TerrainGo = TerrainGenerator.Generate(m_sTerrainName, m_currentSetting, m_TerrainHeighMat);
		}

		//保存
		if (GUILayout.Button("保存"))
		{
			SaveTerrain();
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
	}

	/// <summary>
	/// 保存/更新地形配置
	/// </summary>
	private void SaveTerrainSetting()
	{
		if (m_currentSetting)//更新配置
		{
			GetSettingData(m_currentSetting);
			EditorUtility.SetDirty(m_currentSetting);
			AssetDatabase.SaveAssets();

			ShowTip($"更新{ m_currentSetting.name }配置成功!");
		}
		else//保存配置
		{
			if (!Directory.Exists(CONFIG_FOLDER_PATH))
				Directory.CreateDirectory(CONFIG_FOLDER_PATH);

			//打开保存对话框
			string path = EditorUtility.SaveFilePanelInProject("保存地形配置", m_sTerrainName, "asset", "请选择保存位置", CONFIG_FOLDER_PATH);
			if (string.IsNullOrEmpty(path))
				return;

			//创建新的SO实例
			TerrainSetting setting = GenerateTerrainSetting();

			AssetDatabase.CreateAsset(setting, path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			m_currentSetting = setting;

			ShowTip($"保存{ m_currentSetting.name }配置成功!");
		}
	}

	/// <summary>
	/// 设置配置数据
	/// </summary>
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

	/// <summary>
	/// 获取配置数据
	/// </summary>
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

	/// <summary>
	/// 生成地形配置
	/// </summary>
	private TerrainSetting GenerateTerrainSetting()
	{
		TerrainSetting setting = ScriptableObject.CreateInstance<TerrainSetting>();

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

		return setting;
	}

	private void SaveTerrain()
	{
		string sPath = PREFAB_FOLDER_PATH + "/Terrains/";
		if (!Directory.Exists(sPath))
			Directory.CreateDirectory(sPath);

		if (m_TerrainGo != null)
		{
			var TerrainGO = TerrainGenerator.CombineChunkMeshes(m_TerrainGo, m_TerrainHeighMat);
			//保存mesh
			string sMeshPath = sPath + m_sTerrainName + "_Mesh" + ".asset";
			AssetDatabase.CreateAsset(TerrainGO.GetComponent<MeshFilter>().sharedMesh, sMeshPath);

			//保存prefab
			string sPrefabPath = sPath + m_sTerrainName + ".prefab";
			PrefabUtility.SaveAsPrefabAsset(TerrainGO, sPrefabPath);

			AssetDatabase.SaveAssets();
    		AssetDatabase.Refresh();
			ShowTip($"保存{ m_sTerrainName }成功!");

			//删除
			DestroyImmediate(m_TerrainGo);
			m_TerrainGo = null;
		}
	}

	/// <summary>
	/// 显示提示
	/// </summary>
	/// <param name="msg"></param>
	private void ShowTip(string msg)
	{
		ShowNotification(new GUIContent(msg), 0.5);
	}
}
