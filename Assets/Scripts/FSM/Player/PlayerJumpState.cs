using Echo.FSM;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
	/// <summary>
	/// 跳起重力
	/// </summary>
	private float m_jumpGravity;
	/// <summary>
	/// 跳跃初始速度
	/// </summary>
	private float m_initialJumpVelocity;

    public PlayerJumpState(PlayerStateMachine stateMachine)
		: base(stateMachine)
	{
		//初始化跳跃参数
		float timeToApex = m_stateMachine.Controller.MaxJumpHeight / 2;
		m_jumpGravity = -2 * m_stateMachine.Controller.MaxJumpHeight / Mathf.Pow(timeToApex, 2);
		m_initialJumpVelocity = 2 * m_stateMachine.Controller.MaxJumpHeight / timeToApex;

		Debug.Log($"JumpGravity:{m_jumpGravity} InitialJumpVelocity:{m_initialJumpVelocity}");
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
		bool isFalling = m_stateMachine.CurrentMoveMentY < 0;
		float fallMult = 2.0f;
		if (isFalling)
		{
			float previousY = m_stateMachine.CurrentMoveMentY;
			m_stateMachine.CurrentMoveMentY = m_jumpGravity * fallMult * Time.deltaTime;
			m_stateMachine.AppliedMovmentY = Mathf.Max(previousY + m_stateMachine.CurrentMoveMentY * .5f, -20f);
		}
		else
		{
			float previousY = m_stateMachine.CurrentMoveMentY;
			m_stateMachine.CurrentMoveMentY = m_jumpGravity * Time.deltaTime;
			m_stateMachine.AppliedMovmentY = previousY + m_stateMachine.CurrentMoveMentY * .5f;
		}
		CheckSwitchStates();
	}

	public override void InitSubStates() {}

    public override void CheckSwitchStates()
	{
		if (m_stateMachine.Controller.IsGrounded)
		{
			SwitchState(PlayerStateEnum.Grounded);
		}
	}

	private void HandleJump()
	{
		m_stateMachine.IsJump = true;
		m_stateMachine.CurrentMoveMentY = m_initialJumpVelocity;
		m_stateMachine.AppliedMovmentY = m_initialJumpVelocity;
	}
}
