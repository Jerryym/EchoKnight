using System;
using Echo;
using Echo.Command;
using UnityEngine;

/// <summary>
/// 玩家控制器
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	/// <summary>
	/// 重力
	/// </summary>
	public float gravity = Physics.gravity.y;
	/// <summary>
	/// 地面重力
	/// </summary>
	public float groundGravity = -0.05f;
	/// <summary>
	/// 每帧旋转速度
	/// </summary>
	[Tooltip("每帧旋转速度")]
	public float rotationFactorPerFrame = 15.0f;

	#region 组件
	private CharacterController m_characterController;
	private Animator m_animator;
	#endregion

	private CommandManager m_commandMgr = null;
	private PlayerInputSystem m_inputActions = null;
	private PlayerInputAdapter m_inputAdapter = null;
	private PlayerStateMachine m_stateMachine = null;

	#region Animator Param
	private int m_speedHash;
	#endregion

	#region Unity生命周期函数
	private void Awake()
	{
		//获取组件
		m_characterController = GetComponent<CharacterController>();
		m_animator = GetComponentInChildren<Animator>();

		//初始化输入控制
		InitalInputActions();

		//初始化状态机
		m_stateMachine = new PlayerStateMachine(this);

		//Animator Param
		m_speedHash = Animator.StringToHash("moveSpeed");
	}

	private void OnEnable()
	{
		//启动输入控制
		m_inputActions?.Enable();
	}

	private void OnDisable()
	{
		//禁用输入控制
		m_inputActions?.Disable();
	}

	private void Update()
	{
		HandleRotation();
		//更新状态机
		m_stateMachine.Update();
		//更新动画
		UpdateAnimation();
		m_characterController.Move(m_stateMachine.Movment * Time.deltaTime);
	}

	private void FixedUpdate()
	{

	}
	#endregion

	/// <summary>
	/// 移动
	/// </summary>
	/// <param name="input"></param>
	public void Move(Vector2 input)
	{
		m_stateMachine.SetMoveInput(input);
	}

	/// <summary>
	/// 跳跃
	/// </summary>
	public void Jump()
	{
		m_stateMachine.IsJumpPressed = true;
	}

	/// <summary>
	/// 初始化输入控制
	/// </summary>
	private void InitalInputActions()
	{
		m_commandMgr = new CommandManager(this);
		m_inputActions = new PlayerInputSystem();
		m_inputAdapter = new PlayerInputAdapter(m_commandMgr);

		//移动
		m_inputActions.Player.Move.performed += m_inputAdapter.OnMove;
		m_inputActions.Player.Move.canceled += m_inputAdapter.OnMove;

		//跳跃
		m_inputActions.Player.Jump.started += m_inputAdapter.OnJump;
		m_inputActions.Player.Jump.canceled += m_inputAdapter.OnJump;
	}

	/// <summary>
	/// 更新动画
	/// </summary>
	private void UpdateAnimation()
	{
		Vector3 velocity = m_characterController.velocity;
		float speed = new Vector2(velocity.x, velocity.z).magnitude;
		m_animator.SetFloat(m_speedHash, speed);
	}

	/// <summary>
	/// 处理旋转
	/// </summary>
	private void HandleRotation()
	{
		Vector3 posToLookAt = new Vector3(m_stateMachine.CurrentMoveMent.x, 0.0f, m_stateMachine.CurrentMoveMent.z);
		Quaternion currentRot = transform.rotation;
		//移动时旋转
		if (m_stateMachine.IsMoving)
		{
			Quaternion targetRot = Quaternion.LookRotation(posToLookAt);
			transform.rotation = Quaternion.Slerp(currentRot, targetRot, rotationFactorPerFrame * Time.deltaTime);
		}
	}

	#region setter & getter
	/// <summary>
	/// 是否在地面
	/// </summary>
	public bool IsGrounded => m_characterController.isGrounded;
	#endregion
}
