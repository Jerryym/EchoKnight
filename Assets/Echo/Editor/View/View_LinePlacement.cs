using System;
using Echo.Editor.UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor
{
	/// <summary>
	/// 沿线布设
	/// </summary>
	public class View_LinePlacement : EchoElement
	{
		#region 控件
		private ScrollView m_scrollView = null;

		//沿线布设参数
		private ObjectField m_prefabField = null;
		private FloatField m_spacingField = null;
		private FloatField m_offsetStartField = null;
		private FloatField m_offsetEndField = null;
		private Toggle m_randomRotationToggle = null;
		private FloatField m_rotationField = null;
		private Vector2Field m_rotationRangeField = null;

		private Label m_label = null;
		public Label Label => m_label;

		//按钮
		private ButtonField m_selCurve = null;
		private Button m_drawAndPlaceBtn = null;
		private Button m_previewBtn = null;
		private Button m_okBtn = null;
		private Button m_cancelBtn = null;
		#endregion

		#region 事件
		public event Action SelCurves;
		public event Action DrawAndPlaceBtnClick;
		public event Action PreviewBtnClick;
		public event Action OkClick;
		public event Action CancelClick;
		#endregion

		public View_LinePlacement() : base()
		{
			//加载uss
			StyleSheet uss_GroupBox = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Echo/Editor/View/Styles/GroupBox.uss");
			this.styleSheets.Add(uss_GroupBox);
			this.style.fontSize = 12;

			SetElementTitle("沿线布设");
			InitWidget();
		}

		private void InitWidget()
		{
			m_scrollView = new ScrollView(ScrollViewMode.Vertical);
			m_scrollView.style.flexGrow = 1;
			this.Add(m_scrollView);

			InitParams();
			InitButton();
		}

		private void InitParams()
		{
			//放置对象
			GroupBox contentGroup = new GroupBox("布设内容");
			contentGroup.AddToClassList("group-box");
			m_scrollView.Add(contentGroup);

			m_prefabField = new ObjectField("模型");
			m_prefabField.objectType = typeof(GameObject);
			m_prefabField.RegisterValueChangedCallback(OnPrefabChanged);
			contentGroup.Add(m_prefabField);

			//沿线分布
			GroupBox distributionGroup = new GroupBox("沿线分布");
			distributionGroup.AddToClassList("group-box");
			m_scrollView.Add(distributionGroup);
			m_spacingField = new FloatField("间隔");
			distributionGroup.Add(m_spacingField);

			m_offsetStartField = new FloatField("起点偏移");
			distributionGroup.Add(m_offsetStartField);

			m_offsetEndField = new FloatField("终点偏移");
			distributionGroup.Add(m_offsetEndField);

			//朝向
			GroupBox orientationGroup = new GroupBox("朝向");
			orientationGroup.AddToClassList("group-box");
			m_scrollView.Add(orientationGroup);

			m_randomRotationToggle = new Toggle("随机旋转");
			orientationGroup.Add(m_randomRotationToggle);

			m_rotationField = new FloatField("旋转角度");
			orientationGroup.Add(m_rotationField);

			m_rotationRangeField = new Vector2Field("旋转角度范围");
			orientationGroup.Add(m_rotationRangeField);
			m_randomRotationToggle.RegisterValueChangedCallback(evt =>
			{
				m_rotationRangeField.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
			});
			m_rotationRangeField.style.display = m_randomRotationToggle.value ? DisplayStyle.Flex : DisplayStyle.None;

			//布设路径
			GroupBox pathGroup = new GroupBox("布设路径");
			pathGroup.AddToClassList("group-box");
			m_scrollView.Add(pathGroup);

			m_selCurve = new ButtonField("选择曲线", "选择曲线");
			m_selCurve.button.clicked += () => SelCurves?.Invoke();
			pathGroup.Add(m_selCurve);

			m_label = new Label("已选曲线：0");
			pathGroup.Add(m_label);
		}

		private void InitButton()
		{
			VisualElement buttonPanel = new VisualElement();
			buttonPanel.style.flexDirection = FlexDirection.Row;
			buttonPanel.style.justifyContent = Justify.SpaceBetween;
			buttonPanel.style.alignItems = Align.Center;
			buttonPanel.style.marginTop = 6;
			buttonPanel.style.marginBottom = 4;
			m_scrollView.Add(buttonPanel);

			VisualElement leftPanel = new VisualElement();
			leftPanel.style.flexDirection = FlexDirection.Row;
			buttonPanel.Add(leftPanel);

			m_drawAndPlaceBtn = new Button();
			m_drawAndPlaceBtn.text = "绘制并布设";
			m_drawAndPlaceBtn.clicked += () => DrawAndPlaceBtnClick?.Invoke();
			leftPanel.Add(m_drawAndPlaceBtn);

			VisualElement rightPanel = new VisualElement();
			rightPanel.style.flexDirection = FlexDirection.Row;
			rightPanel.style.justifyContent = Justify.FlexEnd;
			buttonPanel.Add(rightPanel);

			m_previewBtn = new Button();
			m_previewBtn.text = "预览";
			m_previewBtn.clicked += () => PreviewBtnClick?.Invoke();
			rightPanel.Add(m_previewBtn);

			m_okBtn = new Button();
			m_okBtn.text = "确定";
			m_okBtn.clicked += () => OkClick?.Invoke();
			rightPanel.Add(m_okBtn);

			m_cancelBtn = new Button();
			m_cancelBtn.text = "取消";
			m_cancelBtn.clicked += () => CancelClick?.Invoke();
			rightPanel.Add(m_cancelBtn);
		}

		private void OnPrefabChanged(ChangeEvent<UnityEngine.Object> evt)
		{
		}
	}
}
