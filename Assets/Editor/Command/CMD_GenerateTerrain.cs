namespace Echo.Editor.Command
{
	/// <summary>
	/// 程序化地形生成命令
	/// </summary>
	public class CMD_GenerateTerrain : ICommand
	{
		private View_PTGTool m_view;
		private Controller_PTGTool m_controller;

		public void Execute()
		{
			var activeWindow = RPGEditorToolWindow.ActiveWindow;
			if (activeWindow == null)
				return;

			m_controller?.Dispose();
			activeWindow.RemoveElement();

			m_view = new View_PTGTool();
			m_controller = new Controller_PTGTool(m_view);
			activeWindow.AddElement(m_view);
		}

		public void Undo()
		{
			var activeWindow = RPGEditorToolWindow.ActiveWindow;
			if (activeWindow == null)
				return;

			m_controller?.Dispose();
			activeWindow.RemoveElement();
			
			m_controller = null;
			m_view = null;
		}
	}
}
