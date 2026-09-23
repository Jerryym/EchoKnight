using System.Collections.Generic;
using UnityEngine;

namespace Echo.Utils
{
	public static class GameObjectUtility
	{
		/// <summary>
		/// 获取指定图层上所有指定组件的对象
		/// </summary>
		public static List<T> FindComponentsByLayer<T>(LayerMask layerMask)
			where T : Component
		{
			T[] components = Object.FindObjectsByType<T>(FindObjectsSortMode.None);
			if (components.Length == 0)
				return null;

			List<T> results = new List<T>();
			foreach (var component in components)
			{
				int layerIndex = component.gameObject.layer;
				if ((layerMask.value & (1 << layerIndex)) != 0)
				{
					results.Add(component);
				}
			}

			return results;
		}
	}
}
