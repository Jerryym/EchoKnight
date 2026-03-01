using Echo.Editor.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor
{
	public class View_CSVToSO : VisualElement
	{
		#region 组件
		private ScrollView m_scrollView = null;

		private PathField m_filePathField = null;
		private PopupField<string> m_popupField = null;
		private TableWidget m_table = null;

		private Button m_generateBtn = null;
		private Button m_updateBtn = null;
		#endregion

		#region 事件
		public event Action OnGenerateClicked;
		public event Action OnUpdateClicked;
		#endregion

		private List<Type> m_SOConfigTypes = null;
		private List<string> m_SOConfigNames = null;
		private int m_SOConfigIndex = -1;

		public View_CSVToSO()
		{
			m_SOConfigTypes = new List<Type>();
			m_SOConfigNames = new List<string>();
			InitWidget();
		}

		private void InitWidget()
		{
			//初始化标题栏
			InitTitleBar();

			m_scrollView = new ScrollView(ScrollViewMode.Vertical);
			this.Add(m_scrollView);

			//文件路径
			m_filePathField = new PathField("配置文件(.csv)", PathMode.OpenFile, "csv");
			m_scrollView.Add(m_filePathField);
			//下拉框
			InitCombo();
			//表格
			InitTable();
			//按钮
			InitButton();

			UpdateTableByConfigType(0);
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

			Label titleLabel = new Label("CSV转SO");
			titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
			titleLabel.style.fontSize = 14;

			titleBar.Add(titleLabel);
			this.Add(titleBar);
		}

		private void InitTable()
		{
			m_table = new TableWidget(4, 4);
			m_scrollView.Add(m_table);

			string[] tableTiltles = new string[] { "字段1", "字段2", "字段3", "字段4" };
			m_table.SetHeaderLabels(tableTiltles);
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

			m_popupField = new PopupField<string>("配置类型", m_SOConfigNames.Count > 0 ? m_SOConfigNames : new List<string> { "无可用配置" }, 0);
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

		private void OnConfigTypeChanged(ChangeEvent<string> evt)
		{
			m_SOConfigIndex = m_SOConfigNames.IndexOf(evt.newValue);
			UpdateTableByConfigType(m_SOConfigIndex);
		}

		/// <summary>
		/// 根据配置类型刷新表格
		/// </summary>
		private void UpdateTableByConfigType(int index)
		{
			if (index < 0 || index >= m_SOConfigTypes.Count)
				return;

			var configType = m_SOConfigTypes[index];
			var fields = configType.GetFields(BindingFlags.Public | BindingFlags.Instance);

			List<string> headers = new List<string>();
			foreach (var field in fields)
			{
				headers.Add(field.Name);
			}
			if (headers.Count == 0)
				return;

			m_table.SetTableSize(4, headers.Count);
			m_table.SetHeaderLabels(headers);
		}
	}
}
