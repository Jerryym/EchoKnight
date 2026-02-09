using UnityEditor;
using UnityEngine.UIElements;

public class DockWidget : VisualElement
{
	public new class UxmlFactory : UxmlFactory<DockWidget> {}

	/// <summary>
	/// 标题
	/// </summary>
	private Label m_titleLabel;
	/// <summary>
	/// 最大化按钮
	/// </summary>
	private Button m_maximizeBtn;
	/// <summary>
	/// 关闭按钮
	/// </summary>
	private Button m_closeBtn;
	/// <summary>
	/// 内容
	/// </summary>
	private VisualElement m_content;

	public DockWidget()
	{
		//加载UXML
		VisualTreeAsset uxmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI/DockWidget/DockWidget.uxml");
		uxmlAsset.CloneTree(this);

		//加载uss
		StyleSheet ussAsset = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/UI/DockWidget/DockWidget.uss");
		this.styleSheets.Add(ussAsset);

		m_titleLabel = this.Q<Label>("Title");
		m_maximizeBtn = this.Q<Button>("MaximizeBtn");
		m_closeBtn = this.Q<Button>("CloseBtn");
		m_content = this.Q<VisualElement>("Content");

		m_titleLabel.text = "DockWidget";
	}

	public void SetTitle(string title)
	{
		m_titleLabel.text = title;
	}
}
