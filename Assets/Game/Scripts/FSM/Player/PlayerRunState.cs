using Echo.FSM;
using UnityEngine;

public class PlayerRunState : PlayerSubState
{
	private float m_rRunSpeed = 3.5f;

	public PlayerRunState(PlayerStateMachine stateMachine) : base(stateMachine) { }

	public override void EnterState()
	{
		Debug.Log("SubState: 进入Run状态");
	}

	public override void UpdateState()
	{
		m_stateMachine.PlayerMovment = new Vector3(m_stateMachine.Input.x * m_rRunSpeed, m_stateMachine.VelocityY, m_stateMachine.Input.y * m_rRunSpeed);
	}

	public override void ExitState()
	{
	}

	public override PlayerSubStateType Type => PlayerSubStateType.Run;
}
