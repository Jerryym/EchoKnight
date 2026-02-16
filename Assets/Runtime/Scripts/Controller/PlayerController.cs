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
	/// 物理配置
	/// </summary>
	[SerializeField]
	[Tooltip("物理配置")]
	private CharacterPhysicsConfigSO m_physicsConfig;

	#region 组件
	private CharacterController m_characterController;
	private PhysicsCheck m_physicsCheck;
	private Animator m_animator;
	#endregion

	private CommandManager m_commandMgr = null;
	private PlayerInputSystem m_inputActions = null;
	private PlayerInputAdapter m_inputAdapter = null;
	private PlayerStateMachine m_stateMachine = null;

	#region Animator Param
	private int m_moveSpeedHash;
	private int m_iJumpHash;
	private int m_iVelocityYHash;
	#endregion

	private Transform m_cameraTrans;
	private Vector3 m_moveDir;

	#region Unity生命周期函数
	private void Awake()
	{
		//获取组件
		m_characterController = GetComponent<CharacterController>();
		m_physicsCheck = GetComponent<PhysicsCheck>();
		m_animator = GetComponentInChildren<Animator>();

		//初始化输入控制
		InitalInputActions();

		//初始化状态机
		m_stateMachine = new PlayerStateMachine(this);

		//Animator Param
		m_moveSpeedHash = Animator.StringToHash("moveSpeed");
		m_iJumpHash = Animator.StringToHash("isJump");
		m_iVelocityYHash = Animator.StringToHash("velocityY");

		m_cameraTrans = Camera.main.transform;

		//设置鼠标锁定
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
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
		//地面检测
		m_physicsCheck.CheckGround();
		//更新状态机
		m_stateMachine.Update();
		//更新动画
		UpdateAnimation();
		m_characterController.Move(3.5f * Time.deltaTime * m_moveDir);
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
		m_inputActions.Player.Jump.performed += m_inputAdapter.OnJump;
		m_inputActions.Player.Jump.canceled += m_inputAdapter.OnJump;
	}

	/// <summary>
	/// 更新动画
	/// </summary>
	private void UpdateAnimation()
	{
		Vector3 velocity = m_characterController.velocity;
		float speed = new Vector2(velocity.x, velocity.z).magnitude;
		m_animator.SetFloat(m_moveSpeedHash, speed);
		//m_animator.SetBool(m_iJumpHash, m_stateMachine.IsJump);
		//m_animator.SetFloat(m_iVelocityYHash, m_stateMachine.VelocityY);
	}

	/// <summary>
	/// 处理旋转
	/// </summary>
	private void HandleRotation()
	{
		//Vector3 posToLookAt = new Vector3(m_stateMachine.Input.x, 0.0f, m_stateMachine.Input.y);
		//Quaternion currentRot = transform.rotation;
		////移动时旋转
		//if (m_stateMachine.IsMoving)
		//{
		//	Quaternion targetRot = Quaternion.LookRotation(posToLookAt);
		//	transform.rotation = Quaternion.Slerp(currentRot, targetRot, m_physicsConfig.rotationFactorPerFrame * Time.deltaTime);
		//}

		Vector3 cameraForward = m_cameraTrans.forward;
		Vector3 cameraRight = m_cameraTrans.right;
		cameraForward.y = 0f;
		cameraRight.y = 0f;
		cameraForward.Normalize();
		cameraRight.Normalize();

		// 计算世界空间移动方向
		Vector2 input = m_stateMachine.Input;
		m_moveDir = (cameraForward * input.y + cameraRight * input.x).normalized;
		if (m_moveDir.magnitude > 0.1f)
		{
			Quaternion targetRot = Quaternion.LookRotation(m_moveDir, Vector3.up);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, m_physicsConfig.rotationFactorPerFrame * Time.deltaTime);
		}
	}

	#region setter & getter
	public float Gravity => m_physicsConfig.gravity;
	public float GroundGravity => m_physicsConfig.groundGravity;
	public float RotationFactorPerFrame => m_physicsConfig.rotationFactorPerFrame;
	public float MaxJumpHeight => m_physicsConfig.maxJumpHeight;
	public float MaxJumpTime => m_physicsConfig.maxJumpTime;

	public bool IsGrounded
	{
		get
		{
			//上升阶段强制不接地
			if (m_stateMachine.IsJump && m_stateMachine.VelocityY > 0.01f)
			{
				return false;
			}
			return m_physicsCheck.IsGrounded;
		}
	}
	#endregion
}
