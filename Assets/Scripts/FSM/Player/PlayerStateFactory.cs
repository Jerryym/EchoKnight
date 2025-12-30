using UnityEngine;

namespace Echo.FSM
{
	/// <summary>
	/// 玩家状态工厂类: 负责统一管理和实例化所有的玩家状态
	/// </summary>
	public class PlayerStateFactory
	{
		public PlayerStateFactory() { }

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
				case PlayerStateEnum.Location:
					return new PlayerLocationState();
				case PlayerStateEnum.Idle:
					return new PlayerIdleState();
				case PlayerStateEnum.Walk:
					return new PlayerWalkState();
				case PlayerStateEnum.Run:
					return new PlayerRunState();
				default:
					Debug.LogWarning($"未在工厂中定义状态: {stateEnum}");
					break;
			}
			return null;
		}
	}
}
