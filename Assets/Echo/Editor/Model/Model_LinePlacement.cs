using System;
using UnityEngine;

namespace Echo.Editor
{
	[Serializable]
	public class Model_LinePlacement
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string name;
		/// <summary>
		/// 布设模型
		/// </summary>
		public GameObject placeModel;
		/// <summary>
		/// 沿线布设参数
		/// </summary>
		public LinePlacementParam param = new LinePlacementParam();
	}
}
