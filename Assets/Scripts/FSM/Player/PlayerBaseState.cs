namespace Echo.FSM
{
	/// <summary>
	/// 玩家状态基类
	/// </summary>
	public abstract class PlayerBaseState : IState
	{
		public PlayerBaseState() { }

		public abstract void EnterState();
		public abstract void UpdateState();
		public abstract void ExitState();
		/// <summary>
		/// 检查状态切换
		/// </summary>
		public abstract void CheckSwitchStates();
		/// <summary>
		/// 初始化子状态
		/// </summary>
		public abstract void InitSubStates();
	}
}
