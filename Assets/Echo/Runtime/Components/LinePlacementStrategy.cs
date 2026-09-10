using System.Collections.Generic;
using UnityEngine;

namespace Echo.Component
{
	/// <summary>
	/// 沿线布设组件
	/// </summary>
	public class LinePlacementStrategy : MonoBehaviour
	{
		/// <summary>
		/// 关联曲线
		/// </summary>
		[SerializeField]
		private Curve m_curve = null;
		public Curve Curve
		{
			get { return m_curve; }
			set { m_curve = value; }
		}

		/// <summary>
		/// 沿线布设参数
		/// </summary>
		private LinePlacementParam m_param = null;
		public LinePlacementParam Param
		{
			get { return m_param; }
			set { m_param = value; }
		}

		private List<GameObject> m_models = null;

		public LinePlacementStrategy()
		{
			m_param = new LinePlacementParam();
			m_models = new List<GameObject>();
		}
	}
}
