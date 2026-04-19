using Echo.Editor.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor
{
	public class View_CSVToSO : EchoElement
	{
		#region 组件
		private ScrollView m_scrollView = null;

		private PopupField<string> m_popupField = null;
		private ObjectField m_csvObjectField = null;
		private PathField m_configSavePath = null;
		private TableWidget m_table = null;

		private Button m_generateBtn = null;
		private Button m_updateBtn = null;
		#endregion

		#region 事件
		public event Action<TextAsset> OnCSVFileChanged;
		public event Action<Type> OnConfigChanged;
		public event Action OnGenerateClicked;
		public event Action OnUpdateClicked;
		#endregion

		private readonly RPGEditorToolWindow activeWindow = RPGEditorToolWindow.ActiveWindow;

		private List<Type> m_SOConfigTypes = null;
		public IReadOnlyList<Type> SOCofigTypes => m_SOConfigTypes;

		private List<string> m_SOConfigNames = null;
		private int m_SOConfigIndex = 0;

		public string ConfigSavePath => m_configSavePath.Path;

		public View_CSVToSO() : base()
		{
			m_SOConfigTypes = new List<Type>();
			m_SOConfigNames = new List<string>();
			SetElementTitle("CSV转SO");
			InitWidget();
		}

		public void InitData(Model_CSVToSO model)
		{
			m_csvObjectField.value = model.csvfileAsset;
			m_configSavePath.Path = model.configSavePath;
			m_SOConfigIndex = model.configIndex;
		}

		public Model_CSVToSO GetData()
		{
			return new Model_CSVToSO
			{
				csvfileAsset = m_csvObjectField.value as TextAsset,
				configSavePath = m_configSavePath.Path,
				configIndex = m_SOConfigIndex
			};
		}

		public void ClearTable()
		{
			m_table.Reset();
			m_table.style.display = DisplayStyle.None;
		}

		public void UpdateTable(List<TableWidget.ColumnItem> columnItems, TableModel model)
		{
			m_table.Reset();
			m_table.SetColumns(columnItems);
			m_table.SetModel(model);

			//显示表格
			m_table.style.display = DisplayStyle.Flex;
		}

		public void GetTableData(out TableModel model)
		{
			model = m_table.Model;
		}

		private void InitWidget()
		{
			m_scrollView = new ScrollView(ScrollViewMode.Vertical);
			this.Add(m_scrollView);

			//下拉框
			InitCombo();

			//SO保存路径
			m_configSavePath = new PathField("SO存储路径");
			m_scrollView.Add(m_configSavePath);

			//CSV文件
			m_csvObjectField = new ObjectField("配置文件");
			m_csvObjectField.objectType = typeof(TextAsset);
			m_csvObjectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnCSVAssetChanged);
			m_scrollView.Add(m_csvObjectField);

			//表格
			m_table = new TableWidget();
			m_table.style.display = DisplayStyle.None;// 默认隐藏
			m_table.style.flexGrow = 1;
			m_scrollView.Add(m_table);

			//按钮
			InitButton();
		}

		private void InitCombo()
		{
			var types = TypeCache.GetTypesDerivedFrom<ScriptableObject>();
			foreach (var item in types)
			{
				var configAttr = item.GetCustomAttribute<ConfigInfoAttribute>();
				if (configAttr == null)
					continue;

				m_SOConfigTypes.Add(item);
				m_SOConfigNames.Add(configAttr.ConfigName);
			}

			m_popupField = new PopupField<string>("配置类型", m_SOConfigNames.Count > 0 ? m_SOConfigNames : new List<string>(), m_SOConfigIndex);
			m_popupField.RegisterCallback<ChangeEvent<string>>(OnConfigTypeChanged);
			m_scrollView.Add(m_popupField);
		}

		private void InitButton()
		{
			VisualElement buttonPanel = new VisualElement();
			buttonPanel.style.flexDirection = FlexDirection.Row;
			buttonPanel.style.justifyContent = Justify.FlexEnd;
			m_scrollView.Add(buttonPanel);

			m_generateBtn = new Button();
			m_generateBtn.text = "生成";
			m_generateBtn.clicked += () => OnGenerateClicked?.Invoke();
			buttonPanel.Add(m_generateBtn);

			m_updateBtn = new Button();
			m_updateBtn.text = "更新配置";
			m_updateBtn.clicked += () => OnUpdateClicked?.Invoke();
			buttonPanel.Add(m_updateBtn);
		}

		private void OnCSVAssetChanged(ChangeEvent<UnityEngine.Object> evt)
		{
			var csvAsset = evt.newValue as TextAsset;
			if (csvAsset == null)
			{
				ClearTable();
				return;
			}

			//文件类型检测
			string fileName = AssetDatabase.GetAssetPath(csvAsset);
			if (!fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
			{
				RPGEditorToolWindow.ShowTip($"所选文件 \"{csvAsset.name}\" 不是.csv 文件，请重新选择。", StatusBar.TipLevel.Warning);
				
				m_csvObjectField.SetValueWithoutNotify(null);
				return;
			}
			OnCSVFileChanged?.Invoke(csvAsset);
		}

		private void OnConfigTypeChanged(ChangeEvent<string> evt)
		{
			string selectedName = evt.newValue;
			m_SOConfigIndex = m_SOConfigNames.IndexOf(selectedName);
			if (m_SOConfigIndex < 0)
				return;

			OnConfigChanged?.Invoke(m_SOConfigTypes[m_SOConfigIndex]);
		}
	}
}
