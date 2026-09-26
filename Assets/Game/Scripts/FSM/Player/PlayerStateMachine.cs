using Echo.FSM;
using System;
using System.Collections.Generic;
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
	/// 状态字典
	/// </summary>
	private Dictionary<PlayerStateType, PlayerParentState> m_stateDic;
	/// <summary>
	/// 玩家当前状态
	/// </summary>
	private PlayerBaseState m_currentState = null;

	/// <summary>
	/// 输入向量
	/// </summary>
	private Vector2 m_input = Vector2.zero;
	/// <summary>
	/// 移动应用向量
	/// </summary>
	private Vector3 m_playerMovement = Vector3.zero;
	/// <summary>
	/// 垂直方向速度
	/// </summary>
	private float m_rVelocityY = 0.0f;

	private bool m_bIsMoving = false;
	private bool m_bIsRun = false;
	private bool m_bIsJumpPressed = false;
	private bool m_bIsJump = false;

	public PlayerStateMachine(PlayerController playerController)
	{
		m_controller = playerController;

		//初始化状态机
		InitStateMachine();
		m_currentState = m_stateDic[PlayerStateType.Grounded];
		m_currentState.EnterState();
	}

	public void SetMoveInput(Vector2 input)
	{
		m_bIsMoving = input.x != 0 || input.y != 0;
		m_bIsRun = Mathf.Abs(input.x) > 0.5f || Mathf.Abs(input.y) > 0.5f;
		m_input = input;
	}

	/// <summary>
	/// 更新状态
	/// </summary>
	public void Update()
	{
		m_currentState.UpdateState();
		m_playerMovement.y = m_rVelocityY;
	}

	/// <summary>
	/// 切换状态
	/// </summary>
	/// <param name="stateType"></param>
	public void SwitchState(PlayerStateType stateType)
	{
		if (!m_stateDic.TryGetValue(stateType, out var newState))
		{
			Debug.LogError($"状态 {stateType} 未注册！");
			return;
		}

		m_currentState.ExitState();
		m_currentState = newState;
		m_currentState.EnterState();
	}

	/// <summary>
	/// 初始化状态机
	/// </summary>
	private void InitStateMachine()
	{
		m_stateDic = new Dictionary<PlayerStateType, PlayerParentState>();
		//Grounded
		m_stateDic.Add(PlayerStateType.Grounded, new PlayerGroundedState(this));
		//Airborne
		m_stateDic.Add(PlayerStateType.Airborne, new PlayerAirborneState(this));
	}

	#region setter & getter
	public PlayerController Player => m_controller;

	public PlayerBaseState CurrentState
	{
		get { return m_currentState; }
		set { m_currentState = value; }
	}

	public Vector2 Input => m_input;
	public Vector3 PlayerMovment
	{
		get { return m_playerMovement; }
		set { m_playerMovement = value; }
	}
	public float VelocityY
	{
		get { return m_rVelocityY; }
		set { m_rVelocityY = value; }
	}

	public bool IsMoving => m_bIsMoving;
	public bool IsRun => m_bIsRun;
	public bool IsJumpPressed
	{
		get { return m_bIsJumpPressed; }
		set { m_bIsJumpPressed = value; }
	}
	public bool IsJump
	{
		get { return m_bIsJump; }
		set { m_bIsJump = value; }
	}
	#endregion
}
