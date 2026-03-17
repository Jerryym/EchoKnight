using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
	/// <summary>
	/// 命中管理器
	/// </summary>
	public static class PickManager
	{
		private static readonly List<(GameObject go, IPickable sel)> m_pickables = new();
		public static IReadOnlyList<(GameObject go, IPickable sel)> Pickables => m_pickables;

		public static void Register(GameObject go, IPickable sel)
		{
			if (!m_pickables.Exists(x => x.go == go))
				m_pickables.Add((go, sel));
		}

		public static void Unregister(GameObject go)
		{
			if (m_pickables.Exists(x => x.go == go))
				m_pickables.RemoveAll(x => x.go == go);
		}
	}
}
