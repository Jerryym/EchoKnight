using System.Collections.Generic;

namespace Echo.Editor
{
	/// <summary>
	/// 编辑器命令管理器: 单例类
	/// </summary>
	public class CommandManager
	{
		private static CommandManager s_Instance;

		/// <summary>
		/// 当前命令
		/// </summary>
		private ICommand m_currentCMD;

		private Stack<ICommand> m_undoCMDStake;
		private Stack<ICommand> m_redoCMDStake;

		private CommandManager()
		{
			m_undoCMDStake = new Stack<ICommand>();
			m_redoCMDStake = new Stack<ICommand>();
		}

		public static CommandManager GetInstance()
		{
			if (s_Instance == null)
			{
				s_Instance = new CommandManager();
			}
			return s_Instance;
		}

		public void AddCommand(string cmdName, ICommand command)
		{
		}
	}
}
