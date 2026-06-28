using UnityEngine;

namespace Echo
{
	/// <summary>
	/// 选中接口类
	/// </summary>
	public interface IPickable
	{
		/// <summary>
		/// 探测对象
		/// </summary>
		/// <param name="hitPt">探测点</param>
		/// <returns></returns>
		float HitObject(Vector3 hitPt);

		/// <summary>
		/// 获取选中对象
		/// </summary>
		GameObject GetGameObject();
	}
}
