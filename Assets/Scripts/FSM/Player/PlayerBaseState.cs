using System.Collections.Generic;
using UnityEngine;

namespace Echo.FSM
{
	/// <summary>
	/// 玩家状态基类
	/// </summary>
	public abstract class PlayerBaseState : IState
	{
		protected PlayerStateMachine m_stateMachine;
		/// <summary>
		/// 子状态字典
		/// </summary>
		protected Dictionary<PlayerStateEnum, PlayerBaseState> m_subStates;
		/// <summary>
		/// 当前子状态
		/// </summary>
		protected PlayerBaseState m_currentSubState = null;

		public PlayerBaseState(PlayerStateMachine stateMachine)
		{
			m_stateMachine = stateMachine;
			m_subStates = new Dictionary<PlayerStateEnum, PlayerBaseState>();
		}

		public abstract void EnterState();
		public abstract void UpdateState();
		public abstract void ExitState();
		/// <summary>
		/// 检查状态切换
		/// </summary>
		public abstract void CheckSwitchStates();
		/// <summary>
		/// 初始化子状态
		/// </summary>
		public abstract void InitSubStates();

		/// <summary>
		/// 更新所有状态
		/// </summary>
		public void UpdateStates()
		{
			UpdateState();
			//若当前子状态不为空, 则更新子状态
			m_currentSubState?.UpdateStates();
		}

		/// <summary>
		/// 切换状态
		/// </summary>
		/// <param name="stateEnum">目标状态</param>
		protected void SwitchState(PlayerStateEnum stateEnum)
		{
			Debug.Log($"切换状态: {stateEnum}");
			//退出当前状态
			ExitState();
			//创建并进入新状态
			var newState = m_stateMachine.Factory.Create(stateEnum);
			newState.EnterState();
			//设置当前状态
			m_stateMachine.CurrentState = newState;
		}

		/// <summary>
		/// 切换子状态
		/// </summary>
		/// <param name="stateEnum"></param>
		protected void SwitchSubState(PlayerStateEnum stateEnum)
		{
			if (m_subStates.TryGetValue(stateEnum, out var newState))
			{
				//退出当前状态
				ExitState();
				//进入新状态
				newState.EnterState();
				//设置当前状态
				m_currentSubState = newState;
			}
		}
	}
}
