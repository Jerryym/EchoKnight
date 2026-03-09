using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
	public static class SelectionRegistry
	{
		private static readonly List<(GameObject go, ISelection sel)> m_Selections = new();
		public static IReadOnlyList<(GameObject go, ISelection sel)> Selections => m_Selections;

		public static void Register(GameObject go, ISelection sel)
		{
			if (!m_Selections.Exists(x => x.go == go))
				m_Selections.Add((go, sel));
		}

		public static void Unregister(GameObject go)
		{
			m_Selections.RemoveAll(x => x.go == go);
		}
	}
}
