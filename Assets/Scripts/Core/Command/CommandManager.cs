namespace Echo.Command
{
	/// <summary>
	/// 命令管理器
	/// </summary>
	public class CommandManager
	{
		/// <summary>
		/// 当前命令
		/// </summary>
		private ICommand m_currentCMD;

		/// <summary>
		/// 执行命令
		/// </summary>
		/// <param name="command"></param>
		public void ExecuteCommand(ICommand command)
		{
			if (command == null)
				return;

			m_currentCMD = command;
			m_currentCMD.Execute();
		}
	}
}
