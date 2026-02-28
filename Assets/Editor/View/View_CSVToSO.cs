using Echo.Editor.UI;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor
{
	public class View_CSVToSO : VisualElement
	{
		#region 组件
		private ScrollView m_scrollView = null;

		private PathField m_filePathField = null;
		private TableWidget m_table = null;

		private Button m_generateBtn = null;
		private Button m_updateBtn = null;
		#endregion

		#region 事件
		public event Action OnGenerateClicked;
		public event Action OnUpdateClicked;
		#endregion

		public View_CSVToSO()
		{
			InitWidget();
		}

		private void InitWidget()
		{
			//初始化标题栏
			InitTitleBar();

			m_scrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
			this.Add(m_scrollView);

			//文件路径
			m_filePathField = new PathField("配置文件(.csv)", PathMode.OpenFile, "csv");
			m_scrollView.Add(m_filePathField);

			//表格
			InitTable();

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
	}
}
