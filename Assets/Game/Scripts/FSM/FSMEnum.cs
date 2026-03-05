/// <summary>
/// 枚举: 玩家父状态
/// </summary>
public enum PlayerStateType
{
	None = -1,
	Grounded,
	Airborne,
}

/// <summary>
/// 枚举: 玩家子状态
/// </summary>
public enum PlayerSubStateType
{
	None = -1,

	// === Grounded 子状态 ===
	Idle,
	Walk,
	Run,

	// === Airborne 子状态 ===
	Jump,
	Falling
}
