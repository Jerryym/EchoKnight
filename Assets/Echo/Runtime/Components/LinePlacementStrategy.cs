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
		/// 布设模型
		/// </summary>
		[SerializeField]
		private GameObject m_placeModel = null;
		public GameObject PlaceModel
		{
			get { return m_placeModel; }
			set { m_placeModel = value; }
		}

		/// <summary>
		/// 沿线布设参数
		/// </summary>
		[SerializeField]
		private LinePlacementParam m_param = null;
		public LinePlacementParam Param
		{
			get { return m_param; }
			set { m_param = value; }
		}

		public LinePlacementStrategy()
		{
			m_param = new LinePlacementParam();
		}

		public bool Place()
		{
			if (m_curve == null || m_param == null || m_placeModel == null)
				return false;

			if (m_curve.Type == CurveType.PolyLine)//多段线
			{
				LinePlaceByPolyLine();
			}
			return true;
		}

		private void LinePlaceByPolyLine()
		{
			var points = m_curve.GetPoints(m_param.spacing);

		}
	}
}
