using UnityEditor;
using UnityEngine.UIElements;

namespace Echo.Editor
{
	/// <summary>
	/// 沿线布设
	/// </summary>
	public class View_LinePlacement : VisualElement
	{
		#region 控件
		private FloatField m_spacingField = null;
		private FloatField m_offsetStartField = null;
		private FloatField m_offsetEndField = null;
		private Toggle m_randomRotationToggle = null;
		private FloatField m_rotationField = null;
		private Vector2Field m_rotationRangeField = null;
		#endregion

		public View_LinePlacement()
		{
			InitWidget();
		}

		private void InitWidget()
		{
			m_spacingField = new FloatField("间隔");
			this.Add(m_spacingField);

			m_offsetStartField = new FloatField("起点偏移");
			this.Add(m_offsetStartField);

			m_offsetEndField = new FloatField("终点偏移");
			this.Add(m_offsetEndField);

			m_randomRotationToggle = new Toggle("随机旋转");
			this.Add(m_randomRotationToggle);

			m_rotationField = new FloatField("旋转角度");
			this.Add(m_rotationField);

			m_rotationRangeField = new Vector2Field("旋转角度范围");
			this.Add(m_rotationRangeField);

			m_randomRotationToggle.RegisterValueChangedCallback(evt =>
			{
				m_rotationRangeField.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
			});
			m_rotationRangeField.style.display = m_randomRotationToggle.value ? DisplayStyle.Flex : DisplayStyle.None;
		}
	}
}
