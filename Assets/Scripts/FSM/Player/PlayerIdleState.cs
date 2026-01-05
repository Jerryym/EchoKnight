using UnityEngine;

namespace Echo.FSM
{
	public class PlayerIdleState : PlayerGroundedState
	{
		public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

		public override void EnterState()
		{
			Debug.Log("SubState: 进入Idle状态");
			m_stateMachine.Movment = new Vector3(0, m_stateMachine.CurrentMoveMentY, 0);
		}

		public override void UpdateState()
		{
			CheckSwitchStates();
		}

		public override void ExitState()
		{
			
		}

		public override void CheckSwitchStates()
		{
			if (m_stateMachine.IsMoving)
			{
				SwitchState(m_stateMachine.IsRun ? PlayerStateEnum.Run : PlayerStateEnum.Walk);
			}
		}

		public override void InitSubStates()
		{
		}
	}
}
