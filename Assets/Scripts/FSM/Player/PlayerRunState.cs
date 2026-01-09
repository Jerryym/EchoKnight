using UnityEngine;

namespace Echo.FSM
{
	public class PlayerRunState : PlayerBaseState
	{
		private float m_rRunSpeed = 3.5f;

		public PlayerRunState(PlayerStateMachine stateMachine) : base(stateMachine) { }
		
		public override void EnterState()
		{
			Debug.Log("SubState: 进入Run状态");
		}

		public override void UpdateState()
		{
			CheckSwitchStates();
			m_stateMachine.PlayerMovment = new Vector3(m_stateMachine.Input.x * m_rRunSpeed, m_stateMachine.VelocityY, m_stateMachine.Input.y * m_rRunSpeed);
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
			else if (m_stateMachine.IsMoving && !m_stateMachine.IsRun)
			{
				SwitchState(PlayerStateEnum.Walk);
			}
		}

		public override void InitSubStates()
		{
		}
	}
}
