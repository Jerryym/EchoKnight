using Echo.FSM;
using UnityEngine;

public class PlayerIdleState : PlayerSubState
{
	public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

	public override void EnterState()
	{
		Debug.Log("SubState: 进入Idle状态");
	}

	public override void UpdateState()
	{
	}

	public override void ExitState()
	{

	}

	public override PlayerSubStateType Type => PlayerSubStateType.Idle;
}
