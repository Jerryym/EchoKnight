using Echo.Editor.Tool;
using System.Collections.Generic;
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

			List<GameObject> curveGOs = new List<GameObject>();
			EditorToolManager.SetTool(new SelectionTool("请选择曲线", true, result =>
			{
				Debug.Log(result.Count);
			}));
		}

		public void Undo()
		{
			var activeWindow = RPGEditorToolWindow.ActiveWindow;
			if (activeWindow == null)
				return;
		}
	}
}
