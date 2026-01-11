using Echo.FSM;
using UnityEngine;

public class PlayerFallingState : PlayerSubState
{
	private float m_airControl = 0.6f;

	public PlayerFallingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

	public override void EnterState()
	{
		Debug.Log("SubState: 进入Falling状态");
	}

	public override void ExitState()
	{ 
	}

	public override void UpdateState() 
	{
		Vector2 input = m_stateMachine.Input;
		m_stateMachine.PlayerMovment = new Vector3(input.x * m_airControl, m_stateMachine.VelocityY, input.y * m_airControl);
	}

	public override PlayerSubStateType Type => PlayerSubStateType.Falling;
}
