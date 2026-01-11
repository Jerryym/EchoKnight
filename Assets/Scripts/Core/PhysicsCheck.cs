using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
	public float groundCheckOffset = 0.1f;
	public LayerMask groundLayerMask;

	private CharacterController m_controller;
	/// <summary>
	/// 是否在地面上
	/// </summary>
	private bool m_bIsGrounded = false;

	private void Awake()
	{
		m_controller = GetComponent<CharacterController>();
	}

	private void OnDrawGizmosSelected()
	{
		var characterController = GetComponent<CharacterController>();

		//绘制地面碰撞检测球
		Vector3 origin = transform.position + Vector3.up * (characterController.radius - characterController.skinWidth);
		float rRadius = characterController.radius * 0.9f;
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(origin, rRadius);
	}

	/// <summary>
	/// 地面检测
	/// </summary>
	public void CheckGround()
	{
		//检测起点：从角色底部向上偏移一点（避免嵌入地面）
		Vector3 origin = transform.position + Vector3.up * (m_controller.radius - m_controller.skinWidth);
		float rRadius = m_controller.radius * 0.9f;
		if (Physics.SphereCast(origin, rRadius, Vector3.down, out RaycastHit hit, groundCheckOffset, groundLayerMask))
		{
			m_bIsGrounded = true;
		}
		else
		{
			m_bIsGrounded = false;
		}
	}

	#region setter & getter
	public bool IsGrounded => m_bIsGrounded;
	#endregion
}
