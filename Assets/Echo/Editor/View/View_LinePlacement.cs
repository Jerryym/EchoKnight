using Echo.Editor.UI;
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

		//按钮
		private Button m_previewBtn = null;
		private Button m_okBtn = null;
		private	Button m_cancelButton = null;
		#endregion

		public View_LinePlacement() : base()
		{
			SetElementTitle("沿线布设");
			InitWidget();
		}

		private void InitWidget()
		{
			m_scrollView = new ScrollView(ScrollViewMode.Vertical);
			this.Add(m_scrollView);

			InitParams();
			InitButton();
		}

		private void InitParams()
		{
			m_prefabField = new ObjectField("模型");
			m_prefabField.objectType = typeof(GameObject);
			m_prefabField.RegisterValueChangedCallback(OnPrefabChanged);
			m_scrollView.Add(m_prefabField);

			m_spacingField = new FloatField("间隔");
			m_scrollView.Add(m_spacingField);

			m_offsetStartField = new FloatField("起点偏移");
			m_scrollView.Add(m_offsetStartField);

			m_offsetEndField = new FloatField("终点偏移");
			m_scrollView.Add(m_offsetEndField);

			m_randomRotationToggle = new Toggle("随机旋转");
			m_scrollView.Add(m_randomRotationToggle);

			m_rotationField = new FloatField("旋转角度");
			m_scrollView.Add(m_rotationField);

			m_rotationRangeField = new Vector2Field("旋转角度范围");
			m_scrollView.Add(m_rotationRangeField);

			m_randomRotationToggle.RegisterValueChangedCallback(evt =>
			{
				m_rotationRangeField.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
			});
			m_rotationRangeField.style.display = m_randomRotationToggle.value ? DisplayStyle.Flex : DisplayStyle.None;
		}

		private void InitButton()
		{
			VisualElement buttonPanel = new VisualElement();
			buttonPanel.style.flexDirection = FlexDirection.Row;
			buttonPanel.style.justifyContent = Justify.FlexEnd;
			m_scrollView.Add(buttonPanel);

			m_previewBtn = new Button();
			m_previewBtn.text = "预览";
			buttonPanel.Add(m_previewBtn);

			m_okBtn = new Button();
			m_okBtn.text = "确定";
			buttonPanel.Add(m_okBtn);

			m_cancelButton = new Button();
			m_cancelButton.text = "取消";
			buttonPanel.Add(m_cancelButton);
		}

		private void OnPrefabChanged(ChangeEvent<UnityEngine.Object> evt)
		{
		}
	}
}
