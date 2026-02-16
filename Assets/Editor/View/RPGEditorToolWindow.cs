using Echo.EditorTool.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.EditorTool
{
	public class RPGEditorToolWindow : EditorWindow
	{
		#region 节点
		/// <summary>
		/// 根节点
		/// </summary>
		private VisualElement m_root = null;
		/// <summary>
		/// 主面板节点
		/// </summary>
		private VisualElement m_mainContainer = null;
		/// <summary>
		/// 左侧面板节点
		/// </summary>
		private VisualElement m_leftPanel = null;
		/// <summary>
		/// 中心面板节点
		/// </summary>
		private VisualElement m_centerPanel = null;
		/// <summary>
		/// 右侧面板节点
		/// </summary>
		private VisualElement m_rightPanel = null;
		/// <summary>
		/// 状态栏节点
		/// </summary>
		private VisualElement m_statusBar = null;
		#endregion

		#region 控件
		/// <summary>
		/// 工具箱
		/// </summary>
		private DockWidget m_toolBox = null;

		private DockWidget m_Inspector = null;

		private SceneView m_sceneView = null;
		#endregion

		[MenuItem("Tools/RPG Editor Tool")]
		public static void ShowWindow()
		{
			RPGEditorToolWindow wnd = GetWindow<RPGEditorToolWindow>();
			wnd.titleContent = new GUIContent("RPG EditorTool");
		}

		public void CreateGUI()
		{
			m_root = rootVisualElement;
			m_root.style.flexDirection = FlexDirection.Column;

			//初始化布局
			InitLayout();
			InitWidget();
		}

		private void InitLayout()
		{
			/////////////////////////////////////
			//主面板//////////////////////////////
			/////////////////////////////////////
			var leftSplitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
			var rightSplitView = new TwoPaneSplitView(1, 300, TwoPaneSplitViewOrientation.Horizontal);

			//左侧面板
			m_leftPanel = new VisualElement();
			leftSplitView.Add(m_leftPanel);
			leftSplitView.Add(rightSplitView);

			//中心面板
			m_centerPanel = new VisualElement();
			rightSplitView.Add(m_centerPanel);

			//右侧面板
			m_rightPanel = new VisualElement();
			rightSplitView.Add(m_rightPanel);

			m_mainContainer = leftSplitView;
			m_mainContainer.style.flexGrow = 1;

			m_root.Add(m_mainContainer);

			/////////////////////////////////////
			//状态栏//////////////////////////////
			/////////////////////////////////////
			m_statusBar = new VisualElement();
			m_statusBar.style.height = 20;
			m_statusBar.style.flexDirection = FlexDirection.Row;
			m_statusBar.style.alignItems = Align.Center;
			m_statusBar.style.paddingLeft = 6;
			m_statusBar.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);

			m_root.Add(m_statusBar);
		}

		private void InitWidget()
		{
			//工具箱
			m_toolBox = new DockWidget();
			m_toolBox.SetTitle("工具箱");
			m_toolBox.style.flexGrow = 1;
			m_leftPanel.Add(m_toolBox);

			//Scene View
			m_sceneView = EditorWindow.GetWindow<SceneView>();

			//属性栏
			m_Inspector = new DockWidget();
			m_Inspector.SetTitle("属性栏");
			m_Inspector.style.flexGrow = 1;
			m_rightPanel.Add(m_Inspector);
		}

		#region setter & getter
		/// <summary>
		/// 状态栏
		/// </summary>
		public VisualElement StatusBar => m_statusBar;
		#endregion
	}
}
