using Echo.Editor.Command;
using System;
using System.Collections.Generic;

namespace Echo.Editor
{
	/// <summary>
	/// 命令注册类: 单例类
	/// </summary>
	public class CommandRegistry
	{
		private static CommandRegistry s_Instance;
		public static CommandRegistry Instance
		{
			get
			{
				if (s_Instance == null)
					s_Instance = new CommandRegistry();
				return s_Instance;
			}
		}

		private Dictionary<string, Func<ICommand>> m_commandDic;

		/// <summary>
		/// 创建命令
		/// </summary>
		public ICommand Create(string cmdId)
		{
			if (!m_commandDic.TryGetValue(cmdId, out var func))
			{
				return null;
			}
			return func.Invoke();
		}

		private CommandRegistry()
		{
			m_commandDic = new Dictionary<string, Func<ICommand>>();
			RegistryCommands();
		}

		/// <summary>
		/// 注册命令
		/// </summary>
		private void RegistryCommands()
		{
			//绘制工具
			m_commandDic.Add("DrawArc", () => new CMD_DrawArc());
			m_commandDic.Add("DrawPolyline", () => new CMD_DrawPolyline());

			//场景编辑
			m_commandDic.Add("GenerateTerrain", () => new CMD_GenerateTerrain());
			m_commandDic.Add("LinePlacement", () => new CMD_LinePlacement());

			//配置数据
			m_commandDic.Add("CSVToSO", () => new CMD_CSVToSO());
		}

	}
}
