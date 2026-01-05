using UnityEngine;

namespace Echo.FSM
{
	public class PlayerRunState : PlayerGroundedState
	{
		public PlayerRunState(PlayerStateMachine stateMachine) : base(stateMachine) { }
		
		public override void EnterState()
		{
			Debug.Log("SubState: 进入Run状态");
		}

		public override void UpdateState()
		{
			CheckSwitchStates();
			m_stateMachine.Movment = new Vector3(m_stateMachine.CurrentMoveMent.x * 3, m_stateMachine.CurrentMoveMentY, m_stateMachine.CurrentMoveMent.z * 3);
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
