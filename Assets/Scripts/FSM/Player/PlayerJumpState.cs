using Echo.FSM;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
	private float m_initialJumpVelocity;
	private float m_maxJumpHeight = 20.0f;
	private float m_maxJumpTime = 0.5f;

    public PlayerJumpState(PlayerStateMachine stateMachine)
		: base(stateMachine)
	{
	}

	public override void EnterState()
	{
        Debug.Log("进入Jump状态");
		HandleJump();
	}

	public override void ExitState()
	{
	}

    public override void UpdateState()
	{
	}

	public override void InitSubStates()
	{
	}

    public override void CheckSwitchStates()
	{
	}

	private void HandleJump()
	{
		m_stateMachine.IsJump = true;

		float timeToApex = m_maxJumpTime / 2;

	}
}
