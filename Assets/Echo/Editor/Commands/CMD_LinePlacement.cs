using UnityEditor;
using UnityEngine;

namespace Echo.Editor.Command
{
	/// <summary>
	/// 沿线布设命令
	/// </summary>
	public class CMD_LinePlacement : ICommand
	{
		public void Execute()
		{
			var activeWindow = RPGEditorToolWindow.ActiveWindow;
			if (activeWindow == null)
				return;
		}

		public void Undo()
		{
			var activeWindow = RPGEditorToolWindow.ActiveWindow;
			if (activeWindow == null)
				return;
		}
	}
}
