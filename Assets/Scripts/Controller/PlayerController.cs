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
	[Header("配置")]
	/// <summary>
	/// 地面重力
	/// </summary>
	public float groundGravity;
	/// <summary>
	/// 重力
	/// </summary>
	[Range(-10, -1)]
	public float gravity = Physics.gravity.y;
	/// <summary>
	/// 每帧旋转速度
	/// </summary>
	[Tooltip("每帧旋转速度")]
	public float rotationFactorPerFrame = 15.0f;

	# region 组件
	private CharacterController m_characterController;
	private Animator m_animator;
	#endregion

	private CommandManager m_commandMgr = null;
	private PlayerInputSystem m_inputActions = null;
	private PlayerInputAdapter m_inputAdapter = null;

	private Vector3 m_currentMoveMent = Vector3.zero;
	private bool m_isMoving = false;

	#region Animator Param
	private int m_speedHash;
	#endregion

	# region Unity生命周期函数
	private void Awake()
	{
		//获取组件
		m_characterController = GetComponent<CharacterController>();
		m_animator = GetComponentInChildren<Animator>();

		m_commandMgr = new CommandManager(this);
		m_inputActions = new PlayerInputSystem();
		m_inputAdapter = new PlayerInputAdapter(m_commandMgr);

		m_inputActions.Player.Move.performed += m_inputAdapter.OnMove;
		m_inputActions.Player.Move.canceled += m_inputAdapter.OnMove;

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
		HandleGravity();
		HandleRotation();
		UpdateAnimation();
		m_characterController.Move(m_currentMoveMent * Time.deltaTime);
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
		m_isMoving = input.x != 0 || input.y != 0;
		float speed = Math.Abs(input.x) > 0.5f || Math.Abs(input.y) > 0.5f ? 3.0f : 1.0f;
		m_currentMoveMent.x = input.x * speed;
		m_currentMoveMent.z = input.y * speed;
	}

	private void HandleGravity()
	{
		if (m_characterController.isGrounded)
		{
			//在地面上
			m_currentMoveMent.y = groundGravity;
		}
		else
		{
			//不在地面, 则根据重力加速度下降
			m_currentMoveMent.y += gravity * Time.deltaTime;
		}
	}

	private void HandleRotation()
	{
		Vector3 posToLookAt = new Vector3(m_currentMoveMent.x, 0.0f, m_currentMoveMent.z);
		Quaternion currentRot = transform.rotation;
		//移动时旋转
		if (m_isMoving)
		{
			Quaternion targetRot = Quaternion.LookRotation(posToLookAt);
			transform.rotation = Quaternion.Slerp(currentRot, targetRot, rotationFactorPerFrame * Time.deltaTime);
		}
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
}
