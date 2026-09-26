namespace Echo.Command
{
	/// <summary>
	/// 命令管理器
	/// </summary>
	public class CommandManager
	{
		private PlayerController m_player = null;
		/// <summary>
		/// 当前命令
		/// </summary>
		private ICommand m_currentCMD;

		public CommandManager(PlayerController player)
		{
			m_player = player;
		}

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

		#region setter & getter
		public PlayerController Player => m_player;
		#endregion
	}
}
