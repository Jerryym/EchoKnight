namespace Echo.FSM
{
	/// <summary>
	/// 玩家状态基类
	/// </summary>
	public abstract class PlayerBaseState : IState
	{
		protected PlayerStateMachine m_stateMachine;

		public PlayerBaseState(PlayerStateMachine stateMachine)
		{
			m_stateMachine = stateMachine;
		}

		public abstract void EnterState();
		public abstract void UpdateState();
		public abstract void ExitState();
	}
}
