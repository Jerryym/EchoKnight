using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 鼠标管理器
/// </summary>
public class MouseManager : MonoBehaviour
{
	public Transform cameraTrans;

	[Header("Settings")]
	[SerializeField] private float m_sensitivity = 0.1f;
	[SerializeField] private float m_minPitch = -80f;
	[SerializeField] private float m_maxPitch = 80f;

	private float m_yaw;
	private float m_pitch;
	private Vector2 m_lookDelta;

	private void OnEnable()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void OnDisable()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	private void Update()
	{

	}
}
