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
		}

		public List<Pose> Compute()
		{
			if (m_curve == null || m_param == null)
				return null;

			if (m_param.spacing < 1e-6)
			{
				Debug.LogWarning("布设间距不能为0!");
				return null;
			}

			if (m_curve.Type == CurveType.PolyLine)//多段线
			{
				return LinePlaceByPolyLine();
			}
			return null;
		}

		private List<Pose> LinePlaceByPolyLine()
		{
			PolyLine polyLine = m_curve as PolyLine;
			if (polyLine == null)
				return null;

			//获取多段线点坐标（世界坐标系）
			var points = polyLine.GetWorldPoints();
			if (polyLine.Closed)
				points.Add(points[0]);
			
			//计算多段线长度
			float totalLength = GetPolyLineLength(points, out List<float> segmentLengths);

			//获取采样点
			List<Pose> poseList = ComputePoses(points, segmentLengths, totalLength);
			return poseList;
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

		/// <summary>
		/// 按指定间距计算沿线布设 Pose
		/// </summary>
		private List<Pose> ComputePoses(List<Vector3> points, List<float> segmentLengths, float totalLength)
		{
			float accumulation = 0.0f;
			float sampleDistance = 0.0f;
			List<Pose> poseList = new List<Pose>();
			
			for (int i = 0; i < points.Count - 1; i++)
			{
				var pt0 = points[i];
				var pt1 = points[i + 1];
				float length = segmentLengths[i];
				if (length < 1e-6f)
					continue;

				Vector3 forwardVec = (pt1 - pt0).normalized;//前进方向向量
				Vector3 tangentVec = Vector3.Cross(forwardVec, Vector3.up).normalized;
				Quaternion baseRotation = Quaternion.LookRotation(forwardVec, Vector3.up);
				
				while (accumulation + length > sampleDistance)
				{
					//计算采样点
					float lerpT = (sampleDistance - accumulation) / length;
					Vector3 samplePt = Vector3.Lerp(pt0, pt1, lerpT);
					
					//偏移
					float progress = sampleDistance / totalLength;
					float offset = Mathf.Lerp(m_param.offsetStart, m_param.offsetEnd, progress);
					samplePt += tangentVec * offset;

					//旋转
					float angle = m_param.randomRotation ? Random.Range(m_param.rotationRange.x, m_param.rotationRange.y) : m_param.rotation;
					Quaternion rotation = baseRotation * Quaternion.Euler(0f, angle, 0f);//先沿切线朝向，再叠加Y轴旋转

					Pose pose = new Pose(samplePt, rotation);
					poseList.Add(pose);
					
					sampleDistance += m_param.spacing;
				}
				accumulation += length;
			}

			return poseList;
		}
	}
}
