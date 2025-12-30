using Echo.FSM;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
	private PlayerController m_controller;

	private PlayerStateFactory m_factory = null;
	/// <summary>
	/// 玩家状态
	/// </summary>
	private PlayerBaseState m_state = null;

	private void Awake()
	{
		m_controller = GetComponent<PlayerController>();

		//设置状态机
		m_factory = new PlayerStateFactory();
		m_state = m_factory.Create(PlayerStateEnum.Location);
	}
}
