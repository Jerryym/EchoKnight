namespace Echo.Editor
{
	/// <summary>
	/// 编辑器命令接口类
	/// </summary>
    public interface ICommand
    {
		/// <summary>
		/// 执行命令
		/// </summary>
		void Execute();
		/// <summary>
		/// 撤销
		/// </summary>
		void Undo();
    }
}
