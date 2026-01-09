using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
	public float groundCheckOffset = 0.1f;

	private CharacterController m_controller;
	/// <summary>
	/// 是否在地面上
	/// </summary>
	private bool m_bIsGrounded = false;

	private void Awake()
	{
		m_controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		StateCheck();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		
		//绘制地面碰撞检测球
		Vector3 origin = transform.position + (Vector3.up * groundCheckOffset);
		float rRadius = 0.6f;
		Gizmos.DrawSphere(origin, rRadius);
	}

	private void StateCheck()
	{
		//地面检测
		Vector3 origin = transform.position + (Vector3.up * groundCheckOffset);
		float rRadius = m_controller.radius;
		float rGroundedDistance = groundCheckOffset - rRadius + 2 * m_controller.skinWidth;
		if (Physics.SphereCast(origin, rRadius, Vector3.down, out RaycastHit hit, groundCheckOffset))
		{
			m_bIsGrounded = true;
		}
		else
		{
			m_bIsGrounded = false;
		}
		Debug.Log($"地面检测: {m_bIsGrounded}");
	}

	#region setter & getter
	public bool IsGrounded => m_bIsGrounded;
	#endregion
}
