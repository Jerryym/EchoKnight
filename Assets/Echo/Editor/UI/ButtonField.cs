using UnityEngine.UIElements;

namespace Echo.Editor.UI
{
	public class ButtonField : VisualElement
	{
		private Label m_label = null;
		private Button m_button = null;
		public Button button => m_button;

		public ButtonField(string labelText, string btnText)
		{
			//水平布设
			this.style.flexDirection = FlexDirection.Row;

			m_label = new Label(labelText);
			m_button = new Button
			{
				text = btnText
			};

			Add(m_label);
			Add(m_button);
		}
	}
}
