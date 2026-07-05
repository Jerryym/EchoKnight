using Echo.Editor.Tool;

namespace Echo.Editor.Command
{
	/// <summary>
	/// 绘制圆弧
	/// </summary>
	public class CMD_DrawArc : ICommand
	{
		public void Execute()
		{
			EditorToolManager.SetTool(new DrawArcTool());
		}
		
		public void Undo()
		{
		}
	}

	public class CMD_DrawCircle : ICommand
	{
		public void Execute()
		{
			EditorToolManager.SetTool(new DrawCircleTool());
		}

		public void Undo()
		{
		}
	}

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
