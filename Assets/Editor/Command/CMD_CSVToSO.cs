namespace Echo.Editor.Command
{
	public class CMD_CSVToSO : ICommand
	{
		private View_CSVToSO m_view;
		private Controller_CSVToSO m_controller;

		public void Execute()
		{
			var activeWindow = RPGEditorToolWindow.ActiveWindow;
			if (activeWindow == null)
				return;

			m_view = new View_CSVToSO();
			m_controller = new Controller_CSVToSO(m_view);
			activeWindow.AddElement(m_view);
		}

		public void Undo()
		{
		}
	}
}
