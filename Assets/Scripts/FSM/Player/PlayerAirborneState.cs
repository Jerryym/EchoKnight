using Echo.FSM;
using UnityEngine;

public class PlayerAirborneState : PlayerBaseState
{
	public PlayerAirborneState(PlayerStateMachine stateMachine) : base(stateMachine) { }

	public override void EnterState()
	{
		Debug.Log("进入Airborne状态");
	}

	public override void ExitState()
	{
	}

	public override void UpdateState()
	{
	}

	public override void InitSubStates() { }

	public override void CheckSwitchStates()
	{
	}
}
