using Echo.FSM;
using System;
using UnityEngine;

/// <summary>
/// 地面状态：玩家在在地面上的状态
/// 子状态: Idle, Walk, Run
/// </summary>
public class PlayerGroundedState : PlayerParentState
{
	public PlayerGroundedState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
		//初始化子状态
		InitSubStates();
	}

	public override void EnterState()
	{
		Debug.Log("进入Grounded状态");
		m_stateMachine.VelocityY = m_stateMachine.Player.Gravity * Time.deltaTime;
		SwitchSubState(PlayerSubStateType.Idle);
	}

	public override void ExitState()
	{
		m_currentSubState.ExitState();
	}

	public override void UpdateState()
	{
		m_currentSubState.UpdateState();
		CheckSwitchStates();
	}

	protected override void InitSubStates()
	{
		//初始化子状态
		m_subStates.Add(PlayerSubStateType.Idle, new PlayerIdleState(m_stateMachine));
		m_subStates.Add(PlayerSubStateType.Walk, new PlayerWalkState(m_stateMachine));
		m_subStates.Add(PlayerSubStateType.Run, new PlayerRunState(m_stateMachine));
	}

	protected override void CheckSwitchStates()
	{
		Debug.Log($"IsGrounded = {m_stateMachine.Player.IsGrounded}, IsJumpPressed = {m_stateMachine.IsJumpPressed}");
		if (m_stateMachine.IsJumpPressed)//主动跳跃
		{
			m_stateMachine.VelocityY = 8.0f;
			SwitchParentState(PlayerStateType.Airborne);
			return;
		}
		
		if (!m_stateMachine.Player.IsGrounded)//掉落
		{
			SwitchParentState(PlayerStateType.Airborne);
			return;
		}

		//子状态切换
		CheckSwitchSubStates();
	}

	private void CheckSwitchSubStates()
	{
		PlayerSubStateType targetSubState;
		if (!m_stateMachine.IsMoving)
		{
			targetSubState = PlayerSubStateType.Idle;
		}
		else
		{
			targetSubState = m_stateMachine.IsRun ? PlayerSubStateType.Run : PlayerSubStateType.Walk;
		}

		//只有变化时才切换
		if (m_currentSubState == null || m_currentSubState.Type != targetSubState)
		{
			SwitchSubState(targetSubState);
		}
	}
}
