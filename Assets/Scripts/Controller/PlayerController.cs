using Echo;
using Echo.Command;
using UnityEngine;

/// <summary>
/// 玩家控制器
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
	#region Managers
	private CommandManager m_commandMgr = null;
	#endregion

	private PlayerInputSystem m_inputActions = null;
	private PlayerInputAdapter m_inputAdapter = null;

	private void Awake()
	{
		m_commandMgr = new CommandManager();

		m_inputActions = new PlayerInputSystem();
		m_inputAdapter = new PlayerInputAdapter(m_commandMgr);

		m_inputActions.Player.Move.performed += m_inputAdapter.OnMove;
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
	}

	private void FixedUpdate()
	{
		
	}
}
