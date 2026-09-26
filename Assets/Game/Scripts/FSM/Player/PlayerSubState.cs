namespace Echo.FSM
{
	/// <summary>
	/// 子状态
	/// </summary>
	public abstract class PlayerSubState : PlayerBaseState
	{
		public PlayerSubState(PlayerStateMachine stateMachine)
			: base(stateMachine)
		{
		}

		public override void EnterState() { }

		public override void ExitState() { }

		public override void UpdateState() { }

		public abstract PlayerSubStateType Type { get; }
	}
}
