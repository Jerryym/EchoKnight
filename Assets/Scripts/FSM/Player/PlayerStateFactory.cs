using UnityEngine;

namespace Echo.FSM
{
	/// <summary>
	/// 玩家状态工厂类: 负责统一管理和实例化所有的玩家状态
	/// </summary>
	public class PlayerStateFactory
	{
		private PlayerStateMachine m_stateMachine;

		public PlayerStateFactory(PlayerStateMachine stateMachine)
		{
			m_stateMachine = stateMachine;
		}

		/// <summary>
		/// 根据枚举类型创建对应的状态实例
		/// </summary>
		/// <param name="stateEnum">状态枚举标识</param>
		/// <returns>返回派生自 PlayerBaseState 的具体状态对象；如果未匹配则返回 null</returns>
		public PlayerBaseState Create(PlayerStateEnum stateEnum)
		{
			switch(stateEnum)
			{
				case PlayerStateEnum.None:
					return null;
				case PlayerStateEnum.Grounded:
					return new PlayerGroundedState(m_stateMachine);
				case PlayerStateEnum.Idle:
					return new PlayerIdleState(m_stateMachine);
				case PlayerStateEnum.Walk:
					return new PlayerWalkState(m_stateMachine);
				case PlayerStateEnum.Run:
					return new PlayerRunState(m_stateMachine);
				case PlayerStateEnum.Airborne:
					return new PlayerAirborneState(m_stateMachine);
				case PlayerStateEnum.Jump:
					return new PlayerJumpState(m_stateMachine);
				default:
					Debug.LogWarning($"未定义状态: {stateEnum}");
					break;
			}
			return null;
		}
	}
}
