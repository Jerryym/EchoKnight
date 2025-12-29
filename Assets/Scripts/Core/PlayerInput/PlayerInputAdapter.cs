using Echo.Command;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Echo
{
	public class PlayerInputAdapter
	{
		/// <summary>
		/// 命令管理器
		/// </summary>
		private CommandManager m_commandMgr = null;

		public PlayerInputAdapter(CommandManager manager)
		{
			m_commandMgr = manager;
		}

		#region 事件函数
		/// <summary>
		/// 移动
		/// </summary>
		/// <param name="ctx"></param>
		public void OnMove(InputAction.CallbackContext ctx)
		{
			if (!ctx.performed)
				return;

			Vector2 dir = ctx.ReadValue<Vector2>();
			m_commandMgr.ExecuteCommand(new MoveCommand());
		}

		public void OnJump(InputAction.CallbackContext ctx)
		{
			if (!ctx.performed)
				return;

		}
		#endregion
	}
}
