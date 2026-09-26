using System;
using UnityEditor;

namespace Echo.Editor
{
	public class EditorUndoScope : IDisposable
	{
		/// <summary>
		/// Undo组索引
		/// </summary>
		private int m_groupIndex;
		private bool m_isDisposed = false;

		public EditorUndoScope(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Undo组名不可为空!");

			//创建新的Undo Group
			Undo.IncrementCurrentGroup();
			//获取新的Undo组索引
			m_groupIndex = Undo.GetCurrentGroup();
			//设置Undo组名
			Undo.SetCurrentGroupName(name);
		}

		public void Dispose()
		{
			if (m_isDisposed)
				return;

			//整合Undo
			Undo.CollapseUndoOperations(m_groupIndex);
			m_isDisposed = true;
		}
	}
}
