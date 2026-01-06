using UnityEngine;

/// <summary>
/// 玩家物理配置 ScriptableObject: 用于集中管理玩家角色的物理参数
/// </summary>
public class PlayerPhysicsConfigSO : ScriptableObject
{
	/// <summary>
	/// 重力
	/// </summary>
	public float gravity;
	/// <summary>
	/// 在地面上时应用的重力
	/// </summary>
	public float groundGravity;
	/// <summary>
	/// 每帧旋转速度
	/// </summary>
	public float rotationFactorPerFrame;
	/// <summary>
	/// 跳跃初始速度
	/// </summary>
	public float initialJumpVelocity;
	/// <summary>
	/// 最大跳跃高度
	/// </summary>
	public float maxJumpHeight;
	/// <summary>
	/// 最大跳跃持续时间
	/// </summary>
	public float maxJumpTime;
}
