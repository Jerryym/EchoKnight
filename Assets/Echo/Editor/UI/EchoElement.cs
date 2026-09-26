using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor.UI
{
	public abstract class EchoElement : VisualElement
	{
		private Label m_titleLabel = null;

		public EchoElement()
		{
			//标题栏
			InitTitleBar();
		}

		/// <summary>
		/// 设置标题
		/// </summary>
		/// <param name="title">标题文本</param>
		protected void SetElementTitle(string title)
		{
			m_titleLabel.text = title;
		}

		/// <summary>
		/// 初始化标题栏
		/// </summary>
		private void InitTitleBar()
		{
			VisualElement titleBar = new VisualElement();
			titleBar.style.height = 26;
			titleBar.style.justifyContent = Justify.FlexStart;
			titleBar.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
			titleBar.style.paddingLeft = 8;
			titleBar.style.flexDirection = FlexDirection.Row;
			titleBar.style.alignItems = Align.Center;

			m_titleLabel = new Label();
			m_titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
			m_titleLabel.style.fontSize = 14;

			titleBar.Add(m_titleLabel);
			this.Add(titleBar);
		}
	}
}
