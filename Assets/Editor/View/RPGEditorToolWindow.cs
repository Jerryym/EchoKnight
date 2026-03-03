using Echo.Editor.UI;
using Echo.Editor.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor
{
	public class RPGEditorToolWindow : EditorWindow
	{
		public static RPGEditorToolWindow ActiveWindow { get; private set; }

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
		/// 右侧面板节点
		/// </summary>
		private VisualElement m_rightPanel = null;
		/// <summary>
		/// 状态栏节点
		/// </summary>
		private StatusBar m_statusBar = null;
		#endregion

		#region 控件
		/// <summary>
		/// 工具箱
		/// </summary>
		private DockWidget m_toolBox = null;
		/// <summary>
		/// 命令列表控件
		/// </summary>
		private ListView m_commandListView = null;
		#endregion

		/// <summary>
		/// 场景
		/// </summary>
		private SceneView m_sceneView = null;
		/// <summary>
		/// 命令列表
		/// </summary>
		private List<CommandInfo> m_commandList;

		[MenuItem("Tools/RPG Editor Tool")]
		public static void ShowWindow()
		{
			RPGEditorToolWindow wnd = GetWindow<RPGEditorToolWindow>();
			wnd.titleContent = new GUIContent("RPG EditorTool");
		}

		public void CreateGUI()
		{
			ActiveWindow = this;
			//Scene View
			m_sceneView = SceneView.lastActiveSceneView;
			//加载命令XML
			m_commandList = EditorDataLoader.LoadCommandXML("Assets/Editor/Config/Command.xml");

			//初始化布局
			InitLayout();
			InitWidget();
		}

		private void OnGUI()
		{
			Event e = Event.current;
			if (e.type == EventType.KeyDown)
			{
				if (e.control && e.keyCode == KeyCode.Z)
				{
					CommandManager.Instance.Undo();
					e.Use();
				}

				if (e.control && e.keyCode == KeyCode.Y)
				{
					CommandManager.Instance.Redo();
					e.Use();
				}
			}
		}

		public void AddElement(VisualElement element)
		{
			m_rightPanel.Clear();

			element.style.flexGrow = 1;
			m_rightPanel.Add(element);
		}

		public void RemoveElement()
		{
			m_rightPanel.Clear();
		}

		/// <summary>
		/// 显示提示
		/// </summary>
		public static void ShowTip(string text, StatusBar.TipLevel level = StatusBar.TipLevel.Info)
		{
			ActiveWindow.SetStatusBarText(text, level);
		}

		private void OnDestroy()
		{
			if (ActiveWindow == this)
				ActiveWindow = null;
		}

		private void InitLayout()
		{
			m_root = rootVisualElement;
			m_root.style.flexDirection = FlexDirection.Column;

			/////////////////////////////////////
			//主面板//////////////////////////////
			/////////////////////////////////////
			var splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);

			//左侧面板
			m_leftPanel = new VisualElement();
			m_leftPanel.style.flexGrow = 1;
			//右侧面板
			m_rightPanel = new VisualElement();
			m_rightPanel.style.flexGrow = 1;

			splitView.Add(m_leftPanel);
			splitView.Add(m_rightPanel);

			m_mainContainer = splitView;
			m_mainContainer.style.flexGrow = 1;

			m_root.Add(m_mainContainer);

			/////////////////////////////////////
			//状态栏//////////////////////////////
			/////////////////////////////////////
			m_statusBar = new StatusBar();

			m_root.Add(m_statusBar);
		}

		private void InitWidget()
		{
			//工具箱
			m_toolBox = new DockWidget();
			m_toolBox.SetTitle("工具箱");
			m_toolBox.ShowButton(false);
			m_toolBox.style.flexGrow = 1;
			m_leftPanel.Add(m_toolBox);
			InitToolBox();
		}

		/// <summary>
		/// 初始化工具箱
		/// </summary>
		private void InitToolBox()
		{
			//创建列表控件
			m_commandListView = new ListView();
			m_commandListView.style.flexGrow = 1;
			m_commandListView.itemsSource = m_commandList;
			m_commandListView.makeItem = MakeItem;
			m_commandListView.bindItem = BindItem;
			m_commandListView.selectionType = SelectionType.Single;//单选
			m_commandListView.selectionChanged += OnListSelectionChange;

			m_toolBox.SetContent(m_commandListView);
		}

		private VisualElement MakeItem()
		{
			Label cmdLabel = new Label();
			cmdLabel.style.fontSize = 12;
			cmdLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

			return cmdLabel;
		}

		private void BindItem(VisualElement element, int index)
		{
			Label label = element as Label;
			label.text = m_commandList[index].name;
		}

		private void SetStatusBarText(string text, StatusBar.TipLevel level)
		{
			m_statusBar.SetText(text, level);
		}

		#region setter & getter

		#endregion

		#region Event Funcs
		private void OnListSelectionChange(IEnumerable<object> selectedItems)
		{
			foreach (var item in selectedItems)
			{
				var cmdInfo = item as CommandInfo;
				Debug.Log("cmdName = " + cmdInfo.name);
				
				//执行命令
				CommandManager.Instance.Execute(CommandRegistry.Instance.Create(cmdInfo.id));

			}
			
			//取消当前选中
			m_commandListView.ClearSelection();
		}
		#endregion
	}
}
