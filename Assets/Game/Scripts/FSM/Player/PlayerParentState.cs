using System.Collections.Generic;
using UnityEngine;

namespace Echo.FSM
{
	/// <summary>
	/// 父状态
	/// </summary>
	public abstract class PlayerParentState : PlayerBaseState
	{
		/// <summary>
		/// 子状态字典
		/// </summary>
		protected Dictionary<PlayerSubStateType, PlayerSubState> m_subStates;
		/// <summary>
		/// 当前子状态
		/// </summary>
		protected PlayerSubState m_currentSubState;

		public PlayerParentState(PlayerStateMachine stateMachine)
			: base(stateMachine)
		{
			m_subStates = new Dictionary<PlayerSubStateType, PlayerSubState>();
		}

		public override void EnterState() 
		{
		}

		public override void ExitState()
		{
		}

		public override void UpdateState()
		{
		}

		/// <summary>
		/// 初始化子状态
		/// </summary>
		protected abstract void InitSubStates();
		/// <summary>
		/// 检查状态切换
		/// </summary>
		protected abstract void CheckSwitchStates();

		/// <summary>
		/// 切换父状态
		/// </summary>
		/// <param name="type"></param>
		protected void SwitchParentState(PlayerStateType parentStateType)
		{
			m_stateMachine.SwitchState(parentStateType);
		}

		/// <summary>
		/// 切换子状态
		/// </summary>
		/// <param name="subStateType"></param>
		protected void SwitchSubState(PlayerSubStateType subStateType)
		{
			//状态相同, 不切换
			if (m_currentSubState != null && m_currentSubState.Type == subStateType)
				return;

			if (!m_subStates.TryGetValue(subStateType, out var newState))
			{
				Debug.LogWarning($"子状态 {subStateType} 未注册！");
				return;
			}

			m_currentSubState?.ExitState();
			m_currentSubState = newState;
			m_currentSubState.EnterState();
		}
	}
}
