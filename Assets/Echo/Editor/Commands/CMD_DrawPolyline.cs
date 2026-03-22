using Echo.Editor.Tool;

namespace Echo.Editor.Command
{
	/// <summary>
	/// 绘制多段线
	/// </summary>
	public class CMD_DrawPolyline : ICommand
	{
		public void Execute()
		{
			EditorToolManager.SetTool(new DrawPolyLineTool());
		}

		public void Undo()
		{
		}
	}
}
