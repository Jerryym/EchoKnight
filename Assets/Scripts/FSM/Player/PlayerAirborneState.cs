using Echo.FSM;
using UnityEngine;

/// <summary>
/// 空中状态:玩家在空中上的状态,
/// 子状态：Jump, Falling
/// </summary>
public class PlayerAirborneState : PlayerParentState
{
	public PlayerAirborneState(PlayerStateMachine stateMachine)
		: base(stateMachine)
	{
		//初始化子状态
		InitSubStates();
	}

	public override void EnterState()
	{
		Debug.Log("进入Airborne状态");
		SwitchSubState(m_stateMachine.VelocityY > 0.01f ? PlayerSubStateType.Jump : PlayerSubStateType.Falling);
	}

	public override void ExitState()
	{
		m_currentSubState.ExitState();
		m_stateMachine.VelocityY = 0f;
	}

	public override void UpdateState()
	{
		m_stateMachine.VelocityY += m_stateMachine.Player.Gravity * Time.deltaTime;
		m_currentSubState.UpdateState();
		CheckSwitchStates();
	}

	protected override void InitSubStates()
	{
		m_subStates.Add(PlayerSubStateType.Jump, new PlayerJumpState(m_stateMachine));
		m_subStates.Add(PlayerSubStateType.Falling, new PlayerFallingState(m_stateMachine));
	}

	protected override void CheckSwitchStates()
	{
		if (m_stateMachine.Player.IsGrounded)
		{
			SwitchParentState(PlayerStateType.Grounded);
			return;
		}

		//子状态切换
		CheckSwitchSubStates();
	}

	private void CheckSwitchSubStates()
	{
		//Jump -> Falling：当垂直速度 <= 0.01（上升结束，开始下落）
		if (m_currentSubState.Type == PlayerSubStateType.Jump && m_stateMachine.VelocityY <= 0.01f)
		{
			SwitchSubState(PlayerSubStateType.Falling);
		}
	}
}
