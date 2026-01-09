using UnityEngine;

namespace Echo.FSM
{
	public class PlayerWalkState : PlayerBaseState
	{
		private float m_rWalkSpeed = 1.5f;

		public PlayerWalkState(PlayerStateMachine stateMachine)  : base(stateMachine) { }

		public override void EnterState()
		{
			Debug.Log("SubState: 进入Walk状态");
		}

		public override void UpdateState()
		{
			CheckSwitchStates();
			m_stateMachine.PlayerMovment = new Vector3(m_stateMachine.Input.x * m_rWalkSpeed, m_stateMachine.VelocityY, m_stateMachine.Input.y * m_rWalkSpeed);
		}

		public override void ExitState()
		{
		}

		public override void CheckSwitchStates()
		{
			if (!m_stateMachine.IsMoving)
			{
				SwitchState(PlayerStateEnum.Idle);
			}
			else if (m_stateMachine.IsMoving && m_stateMachine.IsRun)
			{
				SwitchState(PlayerStateEnum.Walk);
			}
		}

		public override void InitSubStates()
		{
		}
	}
}
