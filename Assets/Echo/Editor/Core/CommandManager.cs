namespace Echo.Editor
{
	/// <summary>
	/// 编辑器命令管理器: 单例类
	/// </summary>
	public class CommandManager
	{
		private static CommandManager s_Instance;
		public static CommandManager Instance
		{
			get
			{
				if (s_Instance == null)
					s_Instance = new CommandManager();
				return s_Instance;
			}
		}

		/// <summary>
		/// 当前活动命令
		/// </summary>
		private ICommand m_activeCommand = null;

		private CommandManager()
		{
		}

		public void Execute(ICommand command)
		{
			if (command == null)
				return;

			//结束当前命令
			m_activeCommand?.Deactivate();

			//执行命令
			m_activeCommand = command;
			m_activeCommand.Execute();
		}

		public void Deactivate()
		{
			m_activeCommand?.Deactivate();
			m_activeCommand = null;
		}
	}
}
