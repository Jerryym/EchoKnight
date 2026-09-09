namespace Echo.Editor.Command
{
	/// <summary>
	/// 沿线布设
	/// </summary>
	public class CMD_LinePlacement : ICommand
	{
		private View_LinePlacement m_view;
		private Controller_LinePlacement m_controller;

		public void Execute()
		{
			var activeWindow = RPGEditorToolWindow.ActiveWindow;
			if (activeWindow == null)
				return;

			m_controller?.Dispose();
			activeWindow.RemoveElement();

			m_view = new View_LinePlacement();
			m_controller = new Controller_LinePlacement(m_view);
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
