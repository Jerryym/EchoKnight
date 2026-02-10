using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Echo.EditorTool
{
	/// <summary>
	/// 停靠窗口
	/// </summary>
	public class DockWidget : VisualElement
	{
		#region 控件
		/// <summary>
		/// 标题
		/// </summary>
		private Label m_titleLabel;
		/// <summary>
		/// 浮动按钮
		/// </summary>
		private Button m_floatingBtn;
		/// <summary>
		/// 关闭按钮
		/// </summary>
		private Button m_closeBtn;
		/// <summary>
		/// 内容
		/// </summary>
		private VisualElement m_content;
		#endregion

		/// <summary>
		/// 浮动状态标识
		/// </summary>
		private bool m_isFloating = false;

		/// <summary>
		/// UXML工厂
		/// </summary>
		public new class UxmlFactory : UxmlFactory<DockWidget> { }

		public DockWidget()
		{
			LoadUIAsset();
			InitWidget();
			
			//事件绑定
			m_floatingBtn.clicked += OnFloatingButtonClicked;
			m_closeBtn.clicked += OnCloseButtonClicked;
		}

		/// <summary>
		/// 设置标题
		/// </summary>
		/// <param name="title"></param>
		public void SetTitle(string title)
		{
			m_titleLabel.text = title;
		}

		private void LoadUIAsset()
		{
			//加载UXML
			VisualTreeAsset uxmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI/DockWidget/DockWidget.uxml");
			uxmlAsset.CloneTree(this);

			//加载uss
			StyleSheet ussAsset = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/UI/DockWidget/DockWidget.uss");
			this.styleSheets.Add(ussAsset);
		}

		/// <summary>
		/// 初始化控件
		/// </summary>
		private void InitWidget()
		{
			m_titleLabel = this.Q<Label>("Title");
			m_floatingBtn = this.Q<Button>("FloatingBtn");
			m_closeBtn = this.Q<Button>("CloseBtn");
			m_content = this.Q<VisualElement>("Content");

			m_titleLabel.text = "DockWidget";
			m_floatingBtn.tooltip = (m_isFloating) ? "浮动" : "停靠";
			m_closeBtn.tooltip = "关闭";
		}

		#region Event Funcs
		private void OnFloatingButtonClicked()
		{
			m_isFloating = !m_isFloating;

		}

		private void OnCloseButtonClicked()
		{
			this.style.display = DisplayStyle.None;
		}
		#endregion
	}

}
