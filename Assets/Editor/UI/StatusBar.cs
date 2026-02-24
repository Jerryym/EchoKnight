using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor.UI
{
	/// <summary>
	/// 状态栏
	/// </summary>
	public class StatusBar : VisualElement
	{
		/// <summary>
		/// 提示等级
		/// </summary>
		public enum TipLevel
		{
			Info,
			Warning,
			Error
		}

		private readonly Label m_label = null;

		public StatusBar()
		{
			style.height = 20;
			style.flexDirection = FlexDirection.Row;
			style.alignItems = Align.Center;
			style.paddingLeft = 6;
			style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);

			m_label = new Label();
			Add(m_label);
		}

		public void SetText(string text, TipLevel level)
		{
			m_label.text = text;
			switch (level)
			{
				case TipLevel.Info:
					ApplyStyle(new Color(0.18f, 0.18f, 0.18f), new Color(0.85f, 0.85f, 0.85f));
					break;
				case TipLevel.Warning:
					ApplyStyle(new Color(0.35f, 0.28f, 0.0f), new Color(1.0f, 0.8f, 0.2f));
					break;
				case TipLevel.Error:
					ApplyStyle(new Color(0.35f, 0.0f, 0.0f), new Color(1.0f, 0.4f, 0.4f));
					break;
				default:
					break;
			}
		}

		private void ApplyStyle(Color background, Color textColor)
		{
			style.backgroundColor = background;
			m_label.style.color = textColor;
		}
	}
}
