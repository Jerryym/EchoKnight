using System;
using System.Collections.Generic;

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

		private Dictionary<string, Func<ICommand>> m_commandDic;
		private Stack<ICommand> m_undoCMDStake;
		private Stack<ICommand> m_redoCMDStake;

		private CommandManager()
		{
			m_commandDic = new Dictionary<string, Func<ICommand>>();
			m_undoCMDStake = new Stack<ICommand>();
			m_redoCMDStake = new Stack<ICommand>();
		}

		public void Execute(ICommand command)
		{
			command.Execute();
			m_undoCMDStake.Push(command);
			m_redoCMDStake.Clear();
		}

		public void Undo()
		{
			if (m_undoCMDStake.Count == 0)
			{
				return;
			}

			ICommand command = m_undoCMDStake.Pop();
			command.Undo();
			m_redoCMDStake.Push(command);
		}

		public void Redo()
		{
			if (m_redoCMDStake.Count == 0)
			{
				return;
			}

			ICommand command = m_redoCMDStake.Pop();
			command.Execute();
			m_undoCMDStake.Push(command);
		}

	}
}
