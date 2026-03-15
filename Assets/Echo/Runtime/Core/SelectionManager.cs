using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
	/// <summary>
	/// 选择集管理器
	/// </summary>
	public static class SelectionManager
	{
		private static readonly List<(GameObject go, ISelection sel)> m_selections = new();
		public static IReadOnlyList<(GameObject go, ISelection sel)> Selections => m_selections;

		public static void Register(GameObject go, ISelection sel)
		{
			if (!m_selections.Exists(x => x.go == go))
				m_selections.Add((go, sel));
		}

		public static void Unregister(GameObject go)
		{
			if (m_selections.Exists(x => x.go == go))
				m_selections.RemoveAll(x => x.go == go);
		}
	}
}
