using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor.UI
{
	/// <summary>
	/// 消息气泡
	/// </summary>
	public class MessageBubble : VisualElement
	{
		/// <summary>
		/// 文本
		/// </summary>
		private Label m_label = null;

		public MessageBubble(AgentMode role, string text)
		{
			style.flexDirection = FlexDirection.Row;
			style.marginTop = 4;
			style.marginBottom = 4;
			style.marginLeft = 8;
			style.marginRight = 8;

			m_label = new Label(text);
			m_label.style.whiteSpace = WhiteSpace.Normal;
			m_label.style.fontSize = 12;

			if (role == AgentMode.Ask)
			{
				style.justifyContent = Justify.FlexEnd;

				m_label.style.maxWidth = Length.Percent(70);
				m_label.style.color = Color.white;
				m_label.style.backgroundColor = new Color(0.22f, 0.48f, 0.96f);
				m_label.style.paddingLeft = 10;
				m_label.style.paddingRight = 10;
				m_label.style.paddingTop = 6;
				m_label.style.paddingBottom = 6;
				m_label.style.borderTopLeftRadius = 8;
				m_label.style.borderTopRightRadius = 8;
				m_label.style.borderBottomLeftRadius = 8;
				m_label.style.borderBottomRightRadius = 8;
			}
			else
			{
				style.justifyContent = Justify.FlexStart;
				m_label.style.maxWidth = Length.Percent(100);
			}

			Add(m_label);
		}
	}
}
