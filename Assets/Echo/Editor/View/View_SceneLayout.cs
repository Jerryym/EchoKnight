using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor
{
	/// <summary>
	/// 场景布设窗口
	/// </summary>
	public class View_SceneLayout : VisualElement
	{
		#region 组件
		private ScrollView m_scrollView = null;
		#endregion

		#region 事件
		public event Action OnToolButtonClicked;
		#endregion

		private Dictionary<ToolbarButton, PlacementMode> m_toolBtnMap = null;

		private PlacementMode m_currentMode = PlacementMode.None;
		public PlacementMode CurrentMode => m_currentMode;

		public View_SceneLayout()
		{
			m_toolBtnMap = new Dictionary<ToolbarButton, PlacementMode>();
			InitWidget();
		}

		public void InitData()
		{ 
		}

		private void InitWidget()
		{
			//标题栏
			InitTitleBar();

			m_scrollView = new ScrollView(ScrollViewMode.Vertical);
			this.Add(m_scrollView);

			//工具栏
			InitToolBar();
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

			Label titleLabel = new Label("场景布设");
			titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
			titleLabel.style.fontSize = 14;

			titleBar.Add(titleLabel);
			this.Add(titleBar);
		}

		private void InitToolBar()
		{
			VisualElement toolBar = new VisualElement();
			toolBar.style.flexDirection = FlexDirection.Row;
			toolBar.style.justifyContent = Justify.Center;
			toolBar.style.alignItems = Align.Center;
			toolBar.style.paddingBottom = 5;
			toolBar.style.paddingLeft = 8;
			toolBar.style.paddingRight = 6;
			toolBar.style.paddingTop = 5;
			this.Add(toolBar);

			//沿线布设
			ToolbarButton linePlacementBtn = CreateToolButton(PlacementMode.Line, "沿线布设");
			linePlacementBtn.clicked += () => SetPlacementMode(PlacementMode.Line);
			toolBar.Add(linePlacementBtn);

			//区域布设
			ToolbarButton areaPlacementBtn = CreateToolButton(PlacementMode.Area, "区域布设");
			areaPlacementBtn.clicked += () => SetPlacementMode(PlacementMode.Area);
			toolBar.Add(areaPlacementBtn);

			//Shader布设
			ToolbarButton shaderPlacementBtn = CreateToolButton(PlacementMode.Shader, "Shader布设");
			shaderPlacementBtn.clicked += () => SetPlacementMode(PlacementMode.Shader);
			toolBar.Add(shaderPlacementBtn);
		}

		private ToolbarButton CreateToolButton(PlacementMode placementMode, string text)
		{
			ToolbarButton toolbarBtn = new ToolbarButton { text = text };
			toolbarBtn.style.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
			toolbarBtn.style.color = Color.black;

			m_toolBtnMap.Add(toolbarBtn, placementMode);
			return toolbarBtn;
		}

		private void SetPlacementMode(PlacementMode placementMode)
		{
			if (m_currentMode == placementMode)
				return;

			m_currentMode = placementMode;
			UpdateToolButton();
			OnToolButtonClicked?.Invoke();
		}

		private void UpdateToolButton()
		{
			foreach (var item in m_toolBtnMap)
			{
				ToolbarButton button = item.Key;
				bool isSeled = item.Value == m_currentMode;
				button.style.backgroundColor = isSeled ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.8f, 0.8f, 0.8f);
			}
		}
	}
}
