using Echo.FSM;
using UnityEngine;

public class PlayerJumpState : PlayerSubState
{
	/// <summary>
	/// 跳跃初始速度
	/// </summary>
	private float m_rInitialJumpVelocity = 8.0f;

    public PlayerJumpState(PlayerStateMachine stateMachine)
		: base(stateMachine)
	{
	}


	public override void EnterState()
	{
        Debug.Log("SubState: 进入Jump状态");
		m_stateMachine.IsJump = true;
		m_stateMachine.VelocityY = m_rInitialJumpVelocity;
	}

	public override void ExitState()
	{
		m_stateMachine.IsJump = false;
	}

    public override void UpdateState()
	{
		Vector2 input = m_stateMachine.Input;
		float airControl = 0.4f;
		m_stateMachine.PlayerMovment = new Vector3(input.x * airControl, m_stateMachine.VelocityY, input.y * airControl);
	}

	public override PlayerSubStateType Type => PlayerSubStateType.Jump;
}
