using UnityEngine;

namespace Echo.FSM
{
	public class PlayerWalkState : PlayerGroundedState
	{
		public PlayerWalkState(PlayerStateMachine stateMachine)  : base(stateMachine) { }

		public override void EnterState()
		{
			Debug.Log("SubState: 进入Walk状态");
		}

		public override void UpdateState()
		{
			CheckSwitchStates();
			m_stateMachine.Movment = m_stateMachine.CurrentMoveMent;
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
