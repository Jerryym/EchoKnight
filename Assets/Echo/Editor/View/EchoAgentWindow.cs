using Echo.Editor.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor
{
	public class EchoAgentWindow : EditorWindow
	{
		#region 节点
		/// <summary>
		/// 根节点
		/// </summary>
		private VisualElement m_root = null;
		/// <summary>
		/// 历史记录区
		/// </summary>
		private ScrollView m_historyScroll = null;
		#endregion

		#region 控件
		/// <summary>
		/// 输入框
		/// </summary>
		private InputField m_inputField = null;
		#endregion

		[MenuItem("Tools/Echo Agent")]
		public static void ShowWindow()
		{
			EchoAgentWindow wnd = GetWindow<EchoAgentWindow>();
			wnd.titleContent = new GUIContent("Echo Agent");
			wnd.minSize = new Vector2(360, 280);
		}

		public void CreateGUI()
		{
			InitLayout();
		}

		private void InitLayout()
		{
			m_root = rootVisualElement;
			m_root.style.flexDirection = FlexDirection.Column;
			m_root.style.flexGrow = 1;

			/////////////////////////////////////
			//历史记录区//////////////////////////
			/////////////////////////////////////
			m_historyScroll = new ScrollView(ScrollViewMode.Vertical);
			m_historyScroll.style.flexGrow = 1;
			m_root.Add(m_historyScroll);

			/////////////////////////////////////
			//输入框//////////////////////////////
			/////////////////////////////////////
			m_inputField = new InputField();
			m_inputField.Submitted += OnInputSubmitted;
			m_root.Add(m_inputField);
		}

		private void OnInputSubmitted(string text, AgentMode mode)
		{
			MessageBubble bubble = new MessageBubble(AgentMode.Ask, text);
			m_historyScroll.Add(bubble);
			m_historyScroll.schedule.Execute(ScrollHistoryToBottom);
		}

		private void ScrollHistoryToBottom()
		{
			m_historyScroll.scrollOffset = new Vector2(0, m_historyScroll.contentContainer.layout.height);
		}
	}
}
