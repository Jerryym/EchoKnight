using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家控制器
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
	private Animator m_Animator;

	private void Awake()
	{
		m_Animator = GetComponent<Animator>();
	}

	private void Update()
	{
		m_Animator.SetFloat("speed", 0.6f);
	}

	private void FixedUpdate()
	{
		
	}
}
