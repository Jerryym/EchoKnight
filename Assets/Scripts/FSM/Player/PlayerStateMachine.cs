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

		//设置状态机
		m_factory = new PlayerStateFactory(this);
		m_currentState = m_factory.Create(PlayerStateEnum.Grounded);
		m_currentState.EnterState();
	}

	public void SetMoveInput(Vector2 input)
	{
		m_bIsMoving = input.x != 0 || input.y != 0;
		m_bIsRun = Mathf.Abs(input.x) > 0.5f || Mathf.Abs(input.y) > 0.5f;
		m_input = input;
	}

	public void Update()
	{
		m_playerMovement.y = m_rVelocityY * Time.deltaTime;
		//更新状态
		m_currentState.UpdateStates();
	}
	
	#region setter & getter
	public PlayerController Player => m_controller;

	public PlayerBaseState CurrentState
	{
		get { return m_currentState; }
		set { m_currentState = value; }
	}

	public PlayerStateFactory Factory => m_factory;

	public Vector3 Input => m_input;
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
