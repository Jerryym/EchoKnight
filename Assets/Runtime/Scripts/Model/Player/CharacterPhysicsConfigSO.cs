using Echo;
using UnityEngine;

/// <summary>
/// 角色物理配置ScriptableObject: 用于管理角色的物理参数
/// </summary>
[ConfigInfo("角色物理属性")]
public class CharacterPhysicsConfigSO : ScriptableObject
{
	/// <summary>
	/// ID
	/// </summary>
	public string id;
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
	/// 最大跳跃高度
	/// </summary>
	public float maxJumpHeight;
	/// <summary>
	/// 最大跳跃持续时间
	/// </summary>
	public float maxJumpTime;
}
