using System;
using Echo.FSM;
using UnityEngine;

/// <summary>
/// 玩家状态机: 用于存储当前状态、状态相关参数、切换状态状态
/// </summary>
public class PlayerStateMachine
{
	/// <summary>
	/// 玩家控制器
	/// </summary>
	private PlayerController m_controller = null;

	/// <summary>
	/// 状态机工厂
	/// </summary>
	private PlayerStateFactory m_factory = null;
	/// <summary>
	/// 玩家当前状态
	/// </summary>
	private PlayerBaseState m_currentState = null;

	/// <summary>
	/// 当前移动向量
	/// </summary>
	private Vector3 m_currentMoveMent = Vector3.zero;
	/// <summary>
	/// 移动向量
	/// </summary>
	private Vector3 m_Movment = Vector3.zero;

	private bool m_isMoving = false;
	private bool m_isRun = false;

	public PlayerStateMachine(PlayerController playerController)
	{
		m_controller = playerController;

		//设置状态机
		m_factory = new PlayerStateFactory(this);
		m_currentState = m_factory.Create(PlayerStateEnum.Grounded);
		m_currentState.EnterState();
	}

	public void SetMoveInput(Vector2 input)
	{
		m_isMoving = input.x != 0 || input.y != 0;
		m_isRun = Math.Abs(input.x) > 0.5f || Math.Abs(input.y) > 0.5f;

		//设置移动向量
		m_currentMoveMent.x = input.x;
		m_currentMoveMent.z = input.y;
	}

	public void Update()
	{
		//更新状态
		m_currentState.UpdateStates();
	}
	
	#region setter & getter
	public PlayerController Controller => m_controller;

	public PlayerBaseState CurrentState
	{
		get { return m_currentState; }
		set { m_currentState = value; }
	}

	public PlayerStateFactory Factory => m_factory;

	public Vector3 CurrentMoveMent
	{
		get { return m_currentMoveMent; }
		set { m_currentMoveMent = value; }
	}

	public float CurrentMoveMentY
	{
		get { return m_currentMoveMent.y; }
		set { m_currentMoveMent.y = value; }
	}

	public Vector3 Movment
	{
		get { return m_Movment; }
		set { m_Movment = value; }
	}

	public bool IsMoving => m_isMoving;

	public bool IsRun => m_isRun;
	#endregion
}
