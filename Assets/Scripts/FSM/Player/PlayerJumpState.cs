using Echo.FSM;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
	/// <summary>
	/// 跳起重力
	/// </summary>
	private float m_rJumpGravity = 5.0f;
	/// <summary>
	/// 跳跃初始速度
	/// </summary>
	private float m_rInitialJumpVelocity;

    public PlayerJumpState(PlayerStateMachine stateMachine)
		: base(stateMachine)
	{
		//初始化跳跃参数
		//float timeToApex = m_stateMachine.Player.MaxJumpHeight / 2;
		//m_jumpGravity = -2 * m_stateMachine.Player.MaxJumpHeight / Mathf.Pow(timeToApex, 2);
		//m_initialJumpVelocity = 2 * m_stateMachine.Player.MaxJumpHeight / timeToApex;

		Debug.Log($"JumpGravity:{m_rJumpGravity} InitialJumpVelocity:{m_rInitialJumpVelocity}");
	}

	public override void EnterState()
	{
        Debug.Log("进入Jump状态");
		HandleJump();
	}

	public override void ExitState()
	{
		m_stateMachine.IsJumpPressed = false;
		m_stateMachine.IsJump = false;
	}

    public override void UpdateState()
	{
		bool isFalling = m_stateMachine.VelocityY < 0;
		if (isFalling)
		{
			m_stateMachine.VelocityY += m_stateMachine.Player.GroundGravity * Time.deltaTime;
		}
		CheckSwitchStates();
	}

	public override void InitSubStates() {}

    public override void CheckSwitchStates()
	{
		if (m_stateMachine.Player.IsGrounded)
		{
			SwitchState(PlayerStateEnum.Grounded);
		}
	}

	private void HandleJump()
	{
		m_stateMachine.IsJump = true;
		m_stateMachine.VelocityY = m_rJumpGravity;
	}
}
