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

		public List<Vector3> Compute()
		{
			if (m_curve == null || m_param == null)
				return null;

			if (m_curve.Type == CurveType.PolyLine)//多段线
			{
				return LinePlaceByPolyLine();
			}
			return null;
		}

		private List<Vector3> LinePlaceByPolyLine()
		{
			PolyLine polyLine = m_curve as PolyLine;
			if (polyLine == null)
				return null;

			var points = polyLine.GetWorldPoints();
			if (polyLine.Closed)
				points.Add(points[0]);
			float totalLength = GetPolyLineLength(points, out List<float> segmentLengths);

			//获取采样点
			List<Vector3> result = GetPoints(points, segmentLengths, totalLength, polyLine.Closed);


			return result;
		}

		private float GetPolyLineLength(List<Vector3> points, out List<float> segmentLengths)
		{
			float totalLength = 0.0f;
			
			segmentLengths = new List<float>(points.Count - 1);
			for (int i = 0; i < points.Count - 1; i++)
			{
				var pt0 = points[i];
				var pt1 = points[i + 1];
				float length = Vector3.Distance(pt0, pt1);
				segmentLengths.Add(length);
				totalLength += length;
			}
			
			return totalLength;
		}

		private List<Vector3> GetPoints(List<Vector3> points, List<float> segmentLengths, float totalLength, bool isClosed)
		{
			float accumulation = 0.0f;
			float sampleDistance = 0.0f;
			List<Vector3> result = new List<Vector3>();
			for (int i = 0; i < points.Count - 1; i++)
			{
				var pt0 = points[i];
				var pt1 = points[i + 1];
				float length = segmentLengths[i];
				Vector3 tangentVec = (pt0 - pt1).normalized;//切向量
				Vector3 leftVec = Vector3.Cross(tangentVec, Vector3.up).normalized;
				while (accumulation + length > sampleDistance)
				{
					float lerpT = (sampleDistance - accumulation) / length;
					Vector3 samplePt = Vector3.Lerp(pt0, pt1, lerpT);
					float progress = sampleDistance / totalLength;
					float offset = Mathf.Lerp(m_param.offsetStart, m_param.offsetEnd, progress);
					samplePt += leftVec * offset;
					result.Add(samplePt);
					sampleDistance += m_param.spacing;
				}
				accumulation += length;
			}

			return result;
		}
	}
}
