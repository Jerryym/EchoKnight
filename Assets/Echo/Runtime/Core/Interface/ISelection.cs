using UnityEngine;

namespace Echo
{
	/// <summary>
	/// 选中接口类
	/// </summary>
	public interface ISelection
	{
		/// <summary>
		/// 射线检测
		/// </summary>
		bool Raycast(Ray ray, out float distance);

		/// <summary>
		/// 获取选中对象
		/// </summary>
		Object GetObject();
	}
}
