namespace Echo.FSM
{
	/// <summary>
	/// 父状态接口
	/// </summary>
	public interface IState
	{
		/// <summary>
		/// 进入状态
		/// </summary>
		void EnterState();
		/// <summary>
		/// 更新状态
		/// </summary>
		void UpdateState();
		/// <summary>
		/// 退出状态
		/// </summary>
		void ExitState();
	}
}
