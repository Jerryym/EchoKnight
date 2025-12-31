using UnityEngine;

/// <summary>
/// 玩家动画控制器
/// </summary>
public class PlayerAnimationController : MonoBehaviour
{
    private Animator m_animator;

	#region Animator Param
	private int m_speedParam;
	#endregion

	private void Awake()
	{
		m_animator = GetComponent<Animator>();
		m_speedParam = Animator.StringToHash("moveSpeed");
	}

	private void Update()
	{
		m_animator.SetFloat(m_speedParam, 0f);
	}
}
