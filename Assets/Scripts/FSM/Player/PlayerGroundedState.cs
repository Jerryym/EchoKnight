using UnityEngine;

namespace Echo.FSM
{
	/// <summary>
	/// 地面状态：玩家在在地面上的状态, 包含三个子状态——Idle, Walk, Run
	/// </summary>
	public class PlayerGroundedState : PlayerBaseState
	{
		public PlayerGroundedState(PlayerStateMachine stateMachine)
			: base(stateMachine)
		{
			//初始化子状态
			InitSubStates();
		}

		public override void EnterState()
		{
			Debug.Log("进入Grounded状态");
			m_stateMachine.VelocityY = m_stateMachine.Player.GroundGravity;
		}

		public override void ExitState()
		{
		}

		public override void UpdateState()
		{
			CheckSwitchStates();
			HandleGravity();
		}

		public override void InitSubStates()
		{
			//初始化子状态
			m_subStates.Add(PlayerStateEnum.Idle, new PlayerIdleState(m_stateMachine));
			m_subStates.Add(PlayerStateEnum.Walk, new PlayerWalkState(m_stateMachine));
			m_subStates.Add(PlayerStateEnum.Run, new PlayerRunState(m_stateMachine));

			if (!m_stateMachine.IsMoving)
			{
				SwitchSubState(PlayerStateEnum.Idle);
			}
			else
			{
				SwitchSubState(m_stateMachine.IsRun ? PlayerStateEnum.Run : PlayerStateEnum.Walk);
			}
		}

		public override void CheckSwitchStates()
		{
			if (!m_stateMachine.Player.IsGrounded)
			{
				SwitchState(PlayerStateEnum.Airborne);
			}
		}

		/// <summary>
		/// 处理重力
		/// </summary>
		private void HandleGravity()
		{
			if (m_stateMachine.Player.IsGrounded)
			{
				m_stateMachine.VelocityY = m_stateMachine.Player.GroundGravity;
			}
			else
			{
				//不在地面, 则根据重力加速度下降
				m_stateMachine.VelocityY += m_stateMachine.Player.Gravity * Time.deltaTime;
			}
		}
	}
}
