using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor.UI
{
	public enum AgentMode
	{
		Ask,
		Agent
	}

	/// <summary>
	/// 智能体输入框
	/// </summary>
	public class InputField : VisualElement
	{
		#region 节点
		/// <summary>
		/// 底栏
		/// </summary>
		private VisualElement m_toolBar = null;
		#endregion

		#region 控件
		/// <summary>
		/// 文本输入
		/// </summary>
		private TextField m_textField = null;
		/// <summary>
		/// 模式滑块
		/// </summary>
		private VisualElement m_modeSwitch = null;
		/// <summary>
		/// 模式高亮
		/// </summary>
		private VisualElement m_modeThumb = null;
		/// <summary>
		/// Ask 文本
		/// </summary>
		private Label m_askLabel = null;
		/// <summary>
		/// Agent 文本
		/// </summary>
		private Label m_agentLabel = null;
		/// <summary>
		/// 发送按钮
		/// </summary>
		private Button m_sendBtn = null;
		#endregion

		private AgentMode m_mode = AgentMode.Agent;
		public AgentMode Mode => m_mode;

		/// <summary>
		/// 输入文本
		/// </summary>
		public string Text
		{
			get => m_textField.value;
			set => m_textField.value = value;
		}

		/// <summary>
		/// 提交输入。参数为文本和当前模式
		/// </summary>
		public event Action<string, AgentMode> Submitted;

		public InputField()
		{
			style.flexDirection = FlexDirection.Column;
			style.flexShrink = 0;
			style.minHeight = 88;
			style.maxHeight = 160;
			style.marginLeft = 8;
			style.marginRight = 8;
			style.marginTop = 8;
			style.marginBottom = 8;
			style.backgroundColor = new Color(0.18f, 0.18f, 0.18f);
			style.borderTopWidth = 1;
			style.borderBottomWidth = 1;
			style.borderLeftWidth = 1;
			style.borderRightWidth = 1;
			style.borderTopColor = new Color(0.35f, 0.35f, 0.35f);
			style.borderBottomColor = new Color(0.35f, 0.35f, 0.35f);
			style.borderLeftColor = new Color(0.35f, 0.35f, 0.35f);
			style.borderRightColor = new Color(0.35f, 0.35f, 0.35f);
			style.borderTopLeftRadius = 8;
			style.borderTopRightRadius = 8;
			style.borderBottomLeftRadius = 8;
			style.borderBottomRightRadius = 8;
			style.overflow = Overflow.Hidden;

			m_textField = new TextField();
			m_textField.multiline = true;
			m_textField.style.flexGrow = 1;
			m_textField.style.marginLeft = 0;
			m_textField.style.marginRight = 0;
			m_textField.style.marginTop = 0;
			m_textField.style.marginBottom = 0;
			m_textField.style.backgroundColor = Color.clear;
			m_textField.style.borderTopWidth = 0;
			m_textField.style.borderBottomWidth = 0;
			m_textField.style.borderLeftWidth = 0;
			m_textField.style.borderRightWidth = 0;
			m_textField.RegisterCallback<AttachToPanelEvent>(OnTextFieldAttached);
			m_textField.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);
			Add(m_textField);

			m_toolBar = new VisualElement();
			m_toolBar.style.flexDirection = FlexDirection.Row;
			m_toolBar.style.flexShrink = 0;
			m_toolBar.style.height = 32;
			m_toolBar.style.alignItems = Align.Center;
			m_toolBar.style.paddingLeft = 6;
			m_toolBar.style.paddingRight = 6;
			m_toolBar.style.paddingBottom = 6;
			Add(m_toolBar);

			m_modeSwitch = new VisualElement();
			m_modeSwitch.style.flexDirection = FlexDirection.Row;
			m_modeSwitch.style.width = 120;
			m_modeSwitch.style.height = 25;
			m_modeSwitch.style.backgroundColor = new Color(0.12f, 0.12f, 0.12f);
			m_modeSwitch.style.overflow = Overflow.Hidden;
			m_modeSwitch.style.borderTopLeftRadius = 12;
			m_modeSwitch.style.borderTopRightRadius = 12;
			m_modeSwitch.style.borderBottomLeftRadius = 12;
			m_modeSwitch.style.borderBottomRightRadius = 12;
			m_modeSwitch.RegisterCallback<ClickEvent>(OnModeSwitchClicked);
			m_toolBar.Add(m_modeSwitch);

			m_modeThumb = new VisualElement();
			m_modeThumb.style.position = Position.Absolute;
			m_modeThumb.style.left = 0;
			m_modeThumb.style.top = 0;
			m_modeThumb.style.bottom = 0;
			m_modeThumb.style.width = Length.Percent(50);
			m_modeThumb.style.backgroundColor = new Color(0.28f, 0.28f, 0.28f);
			m_modeThumb.style.borderTopLeftRadius = 12;
			m_modeThumb.style.borderTopRightRadius = 12;
			m_modeThumb.style.borderBottomLeftRadius = 12;
			m_modeThumb.style.borderBottomRightRadius = 12;
			m_modeThumb.pickingMode = PickingMode.Ignore;
			m_modeSwitch.Add(m_modeThumb);

			m_askLabel = CreateModeLabel("Ask");
			m_agentLabel = CreateModeLabel("Agent");
			m_modeSwitch.Add(m_askLabel);
			m_modeSwitch.Add(m_agentLabel);
			UpdeateWidget();

			m_sendBtn = new Button();
			m_sendBtn.text = "↑";
			m_sendBtn.style.width = 22;
			m_sendBtn.style.height = 22;
			m_sendBtn.style.marginLeft = StyleKeyword.Auto;
			m_sendBtn.style.marginRight = 0;
			m_sendBtn.style.marginTop = 0;
			m_sendBtn.style.marginBottom = 0;
			m_sendBtn.style.paddingLeft = 0;
			m_sendBtn.style.paddingRight = 0;
			m_sendBtn.style.paddingTop = 0;
			m_sendBtn.style.paddingBottom = 0;
			m_sendBtn.style.unityTextAlign = TextAnchor.MiddleCenter;
			m_sendBtn.style.backgroundColor = new Color(0.22f, 0.48f, 0.96f);
			m_sendBtn.style.color = Color.white;
			m_sendBtn.style.borderTopWidth = 0;
			m_sendBtn.style.borderBottomWidth = 0;
			m_sendBtn.style.borderLeftWidth = 0;
			m_sendBtn.style.borderRightWidth = 0;
			m_sendBtn.style.borderTopLeftRadius = 11;
			m_sendBtn.style.borderTopRightRadius = 11;
			m_sendBtn.style.borderBottomLeftRadius = 11;
			m_sendBtn.style.borderBottomRightRadius = 11;
			m_sendBtn.clicked += OnSendBtnClicked;
			m_toolBar.Add(m_sendBtn);
		}

		private Label CreateModeLabel(string text)
		{
			Label label = new Label(text);
			label.style.flexGrow = 1;
			label.style.unityTextAlign = TextAnchor.MiddleCenter;
			label.style.fontSize = 11;
			label.style.unityFontStyleAndWeight = FontStyle.Bold;
			label.pickingMode = PickingMode.Ignore;
			return label;
		}

		private void OnModeSwitchClicked(ClickEvent evt)
		{
			float localX = evt.localPosition.x;
			m_mode = localX < m_modeSwitch.layout.width * 0.5f ? AgentMode.Ask : AgentMode.Agent;

			UpdeateWidget();
		}

		private void UpdeateWidget()
		{
			bool isAsk = m_mode == AgentMode.Ask;
			m_modeThumb.style.left = isAsk ? 0 : Length.Percent(50);
			m_askLabel.style.color = isAsk ? Color.white : new Color(0.65f, 0.65f, 0.65f);
			m_agentLabel.style.color = isAsk ? new Color(0.65f, 0.65f, 0.65f) : Color.white;
		}

		private void OnTextFieldAttached(AttachToPanelEvent evt)
		{
			VisualElement input = m_textField.Q(className: "unity-text-field__input");
			if (input == null)
				return;

			input.style.backgroundColor = Color.clear;
			input.style.borderTopWidth = 0;
			input.style.borderBottomWidth = 0;
			input.style.borderLeftWidth = 0;
			input.style.borderRightWidth = 0;
			input.style.paddingLeft = 8;
			input.style.paddingRight = 8;
			input.style.paddingTop = 8;
			input.style.paddingBottom = 2;
		}

		private void OnSendBtnClicked()
		{
			Submit();
		}

		private void OnInputKeyDown(KeyDownEvent evt)
		{
			if (evt.keyCode != KeyCode.Return && evt.keyCode != KeyCode.KeypadEnter)
				return;
			if (evt.shiftKey)
				return;

			evt.StopImmediatePropagation();
			evt.PreventDefault();
			Submit();
		}

		private void Submit()
		{
			string text = m_textField.value;
			if (string.IsNullOrWhiteSpace(text))
				return;

			Submitted?.Invoke(text.Trim(), Mode);
			m_textField.schedule.Execute(ClearInput);
		}

		private void ClearInput()
		{
			m_textField.cursorIndex = 0;
			m_textField.selectIndex = 0;
			m_textField.SetValueWithoutNotify("");
		}
	}
}
