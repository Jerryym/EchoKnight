using Echo.Editor.UI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor
{
	/// <summary>
	/// PTG工具窗口
	/// </summary>
	public class View_PTGTool : VisualElement
	{
		private readonly List<string> m_LODOptions = new List<string> { "1级（仅 LOD0）", "2级（LOD0 ~ LOD1）", "3级（LOD0 ~ LOD2）" };
		private int m_LODOptionsIndex = 0;

		#region 控件
		private ScrollView m_scrollView = null;

		private ObjectField m_terrainCfgField = null;
		private PathField m_configSavePath = null;
		private PathField m_terrainPath = null;

		private TextField m_terrainNameField = null;
		private EnumField m_terrainTypeField = null;
		private ObjectField m_terrainMatField = null;
		
		private IntegerField m_terrainWidthField = null;
		private IntegerField m_terrainLengthField = null;
		private IntegerField m_chunkSizeField = null;
		private FloatField m_minHeightField = null;
		private FloatField m_maxHeightField = null;
		private CurveField m_heightCurveField = null;
		
		private FloatField m_noiseScaleFied = null;
		private IntegerField m_octavesField = null;
		private Slider m_persistanceSilder = null;
		private Slider m_lacunaritySilder = null;
		
		private Toggle m_toggleLOD = null;
		private DropdownField m_lodLevelField = null;
		
		private Button m_updateBtn = null;
		private Button m_refreshBtn = null;
		private Button m_saveBtn = null;
		#endregion

		public string ConfigSavePath => m_configSavePath.Path;
		public string TerrainSavePath => m_terrainPath.Path;

		#region 事件
		public event Action OnUpdateClicked;
		public event Action OnRefreshClicked;
		public event Action OnSaveClicked;
		#endregion

		public View_PTGTool()
		{
			//加载uss
			StyleSheet uss_GroupBox = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Echo/Editor/View/Styles/GroupBox.uss");
			this.styleSheets.Add(uss_GroupBox);
			this.style.fontSize = 12;

			InitWidget();
		}

		public void InitData(Model_PTGTool model)
		{
			m_terrainCfgField.value = model.terrainSetting;
			m_configSavePath.Path = model.configSavePath;
			m_terrainPath.Path = model.terrainSavePath;

			m_terrainNameField.value = model.terrainName;
			m_terrainTypeField.value = model.terrainType;
			m_terrainMatField.value = model.terrainMaterial;

			m_terrainWidthField.value = model.terrainWidth;
			m_terrainLengthField.value = model.terrainLength;
			m_chunkSizeField.value = model.chunkSize;
			m_minHeightField.value = model.minHeight;
			m_maxHeightField.value = model.maxHeight;
			m_heightCurveField.value = model.heightCurve;

			m_noiseScaleFied.value = model.noiseScale;
			m_octavesField.value = model.octaves;
			m_persistanceSilder.value = model.persistence;
			m_lacunaritySilder.value = model.lacunarity;

			m_toggleLOD.value = model.enableLOD;
			m_lodLevelField.index = model.lodLevel; 
		}

		/// <summary>
		/// 获取界面数据
		/// </summary>
		/// <returns></returns>
		public Model_PTGTool GetData()
		{
			return new Model_PTGTool
			{
				terrainSetting = m_terrainCfgField.value as TerrainSetting,
				configSavePath = m_configSavePath.Path,
				terrainSavePath = m_terrainPath.Path,

				terrainName = m_terrainNameField.value,
				terrainType = (TerrainType)m_terrainTypeField.value,
				terrainMaterial = m_terrainMatField.value as Material,

				terrainWidth = m_terrainWidthField.value,
				terrainLength = m_terrainLengthField.value,
				chunkSize = m_chunkSizeField.value,
				minHeight = m_minHeightField.value,
				maxHeight = m_maxHeightField.value,
				heightCurve = m_heightCurveField.value,

				noiseScale = m_noiseScaleFied.value,
				octaves = m_octavesField.value,
				persistence = m_persistanceSilder.value,
				lacunarity = m_lacunaritySilder.value,

				enableLOD = m_toggleLOD.value,
				lodLevel = m_lodLevelField.index
			};
		}

		private void InitWidget()
		{
			//初始化标题栏
			InitTitleBar();

			m_scrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
			this.Add(m_scrollView);

			//地形配置文件
			m_terrainCfgField = new ObjectField("地形配置文件");
			m_terrainCfgField.objectType = typeof(TerrainSetting);
			m_terrainCfgField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnTerrainSOChanged);
			m_scrollView.Add(m_terrainCfgField);

			//配置文件保存路径
			m_configSavePath = new PathField("配置文件路径");
			m_configSavePath.tooltip = "TerrainSetting资源保存路径";
			m_scrollView.Add(m_configSavePath);

			//地形保存路径
			m_terrainPath = new PathField("地形输出路径");
			m_terrainPath.tooltip = "生成的地形数据保存位置";
			m_scrollView.Add(m_terrainPath);

			//参数组
			GroupBox paramGroup = new GroupBox("参数");
			paramGroup.AddToClassList("group-box");
			m_scrollView.Add(paramGroup);
			//基本参数
			InitBaseParam(paramGroup);
			//地形参数
			InitTerrainGroup(paramGroup);
			//噪声配置参数
			InitNoiseParamGroup(paramGroup);
			
			//LOD
			InitLODSetting();
			
			//按钮
			InitButton();
		}

		private void InitTitleBar()
		{
			VisualElement titleBar = new VisualElement();
			titleBar.style.height = 26;
			titleBar.style.justifyContent = Justify.FlexStart;
			titleBar.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
			titleBar.style.paddingLeft = 8;
			titleBar.style.flexDirection = FlexDirection.Row;
			titleBar.style.alignItems = Align.Center;

			Label titleLabel = new Label("程序化生成地形工具");
			titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
			titleLabel.style.fontSize = 14;

			titleBar.Add(titleLabel);
			this.Add(titleBar);
		}

		private void InitBaseParam(GroupBox groupBox)
		{
			m_terrainNameField = new TextField("地形名称");
			m_terrainNameField.value = "Terrain";
			groupBox.Add(m_terrainNameField);

			m_terrainTypeField = new EnumField("地形类型", TerrainType.None);
			m_terrainTypeField.value = TerrainType.None;
			m_terrainTypeField.RegisterValueChangedCallback<Enum>(UpdateParamByType);
			groupBox.Add(m_terrainTypeField);

			m_terrainMatField = new ObjectField("地形材质");
			m_terrainMatField.objectType = typeof(Material);
			groupBox.Add(m_terrainMatField);
		}

		private void InitTerrainGroup(GroupBox groupBox)
		{
			Foldout terrainParamGroup = new Foldout { text = "地形参数" };
			groupBox.Add(terrainParamGroup);

			m_terrainWidthField = new IntegerField("地形宽度 X");
			m_terrainWidthField.value = 256;
			terrainParamGroup.Add(m_terrainWidthField);

			m_terrainLengthField = new IntegerField("地形长度 Z");
			m_terrainLengthField.value = 256;
			terrainParamGroup.Add(m_terrainLengthField);

			m_chunkSizeField = new IntegerField("分块大小");
			m_chunkSizeField.value = 32;
			terrainParamGroup.Add(m_chunkSizeField);

			m_minHeightField = new FloatField("地形最小高度");
			m_minHeightField.value = 0;
			terrainParamGroup.Add(m_minHeightField);

			m_maxHeightField = new FloatField("地形最大高度");
			m_maxHeightField.value = 20;
			terrainParamGroup.Add(m_maxHeightField);

			m_heightCurveField = new CurveField("高度曲线");
			m_heightCurveField.value = AnimationCurve.Linear(0, 0, 1, 1);
			terrainParamGroup.Add(m_heightCurveField);
		}

		private void InitNoiseParamGroup(GroupBox groupBox)
		{
			Foldout noiseParamGroup = new Foldout { text = "噪声配置参数" };
			groupBox.Add(noiseParamGroup);

			m_noiseScaleFied = new FloatField("噪声缩放");
			noiseParamGroup.Add(m_noiseScaleFied);

			m_octavesField = new IntegerField("噪声层数");
			noiseParamGroup.Add(m_octavesField);

			m_persistanceSilder = new Slider("持久度", 0f, 1f);
			m_persistanceSilder.showInputField = true;
			m_persistanceSilder.tooltip = "Perlin Noise Persistence";
			noiseParamGroup.Add(m_persistanceSilder);

			m_lacunaritySilder = new Slider("空隙度", 1f, 4f);
			m_lacunaritySilder.showInputField = true;
			m_lacunaritySilder.tooltip = "Perlin Noise Lacunarity";
			noiseParamGroup.Add(m_lacunaritySilder);
		}

		private void InitLODSetting()
		{
			GroupBox groupBox = new GroupBox("LOD设置");
			groupBox.AddToClassList("group-box");
			m_scrollView.Add(groupBox);

			m_toggleLOD = new Toggle("开启LOD");
			m_toggleLOD.value = true;
			groupBox.Add(m_toggleLOD);

			m_lodLevelField = new DropdownField("LOD等级", m_LODOptions, m_LODOptionsIndex);
			m_lodLevelField.index = 0;
			groupBox.Add(m_lodLevelField);

			//注册回调
			m_toggleLOD.RegisterValueChangedCallback(evt =>
			{
				m_lodLevelField.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
			});
			m_lodLevelField.style.display = m_toggleLOD.value ? DisplayStyle.Flex : DisplayStyle.None;
		}

		private void InitButton()
		{
			VisualElement buttonPanel = new VisualElement();
			buttonPanel.style.flexDirection = FlexDirection.Row;
			buttonPanel.style.justifyContent = Justify.FlexEnd;
			m_scrollView.Add(buttonPanel);

			m_updateBtn = new Button();
			m_updateBtn.text = "保存/更新设置";
			m_updateBtn.clicked += () => OnUpdateClicked?.Invoke();
			buttonPanel.Add(m_updateBtn);

			m_refreshBtn = new Button();
			m_refreshBtn.text = "生成/刷新";
			m_refreshBtn.clicked += () => OnRefreshClicked?.Invoke();
			buttonPanel.Add(m_refreshBtn);

			m_saveBtn = new Button();
			m_saveBtn.text = "保存";
			m_saveBtn.clicked += () => OnSaveClicked?.Invoke();
			buttonPanel.Add(m_saveBtn);
		}

		private void UpdateParamByType(ChangeEvent<Enum> evt)
		{
			TerrainType type = (TerrainType)evt.newValue;
			switch (type)
			{
				case TerrainType.Plain:
					m_minHeightField.value = 10f;
					m_maxHeightField.value = 60f;
					m_heightCurveField.value = AnimationCurve.EaseInOut(0, 0, 1, 1);
					m_noiseScaleFied.value = 100f;
					m_octavesField.value = 2;
					m_persistanceSilder.value = 0.4f;
					m_lacunaritySilder.value = 1.8f;
					break;
				case TerrainType.Hill:
					m_minHeightField.value = 60f;
					m_maxHeightField.value = 100f;
					m_heightCurveField.value = AnimationCurve.EaseInOut(0, 0, 1, 1);
					m_noiseScaleFied.value = 60f;
					m_octavesField.value = 4;
					m_persistanceSilder.value = 0.5f;
					m_lacunaritySilder.value = 2.2f;
					break;
				case TerrainType.Mountain:
					m_minHeightField.value = 100f;
					m_maxHeightField.value = 150f;
					m_heightCurveField.value = AnimationCurve.EaseInOut(0, 0, 1, 1);
					m_noiseScaleFied.value = 40f;
					m_octavesField.value = 5;
					m_persistanceSilder.value = 0.6f;
					m_lacunaritySilder.value = 2.2f;
					break;
				default:
					break;
			}
		}

		private void OnTerrainSOChanged(ChangeEvent<UnityEngine.Object> evt)
		{
			var terrainSO = evt.newValue as TerrainSetting;
			if (terrainSO == null)
				return;

			UpdateData(terrainSO);
		}

		private void UpdateData(TerrainSetting terrainSO)
		{
			m_terrainNameField.value = terrainSO.name;
			m_terrainMatField.value = terrainSO.material;

			//地形尺寸
			m_terrainWidthField.value = terrainSO.terrainWidth;
			m_terrainLengthField.value = terrainSO.terrainLength;
			m_chunkSizeField.value = terrainSO.chunkSize;

			//高度设置
			m_minHeightField.value = terrainSO.minHeight;
			m_maxHeightField.value = terrainSO.maxHeight;
			m_heightCurveField.value = terrainSO.heightCurve;

			//噪声参数
			m_noiseScaleFied.value = terrainSO.noiseScale;
			m_octavesField.value = terrainSO.octaves;
			m_persistanceSilder.value = terrainSO.persistence;
			m_lacunaritySilder.value = terrainSO.lacunarity;

			//LOD
			m_toggleLOD.value = terrainSO.enableLOD;
			m_lodLevelField.index = terrainSO.lodLevel - 1;
		}
	}
}
