using Echo;
using UnityEngine;

/// <summary>
/// 角色物理配置ScriptableObject: 用于管理角色的物理参数
/// </summary>
[ConfigInfo("角色物理属性")]
public class CharacterPhysicsConfigSO : ScriptableObject
{
	/// <summary>
	/// 角色类型
	/// </summary>
	[ConfigField("角色类型")]
	public string characterType;
	/// <summary>
	/// 重力
	/// </summary>
	[ConfigField("重力")]
	public float gravity;
	/// <summary>
	/// 在地面上时应用的重力
	/// </summary>
	[ConfigField("地面重力")]
	public float groundGravity;
	/// <summary>
	/// 每帧旋转速度
	/// </summary>
	[ConfigField("每帧旋转速度")]
	public float rotationFactorPerFrame;
	/// <summary>
	/// 最大跳跃高度
	/// </summary>
	[ConfigField("最大跳跃高度")]
	public float maxJumpHeight;
	/// <summary>
	/// 最大跳跃持续时间
	/// </summary>
	[ConfigField("最大跳跃时间")]
	public float maxJumpTime;
}
