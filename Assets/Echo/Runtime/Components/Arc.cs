using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Component
{
	/// <summary>
	/// 圆弧组件
	/// </summary>
	public class Arc : Curve, IPickable
	{
		[SerializeField]
		/// <summary>
		/// 圆心点
		/// </summary>
		private Vector3 m_centerPoint;
		public Vector3 CenterPoint
		{
			get => m_centerPoint;
			set => m_centerPoint = value;
		}
		public Vector3 WorldCenterPoint => transform.TransformPoint(m_centerPoint);
		
		[SerializeField]
		/// <summary>
		/// 半径
		/// </summary>
		private float m_radius;
		public float Radius
		{
			get => m_radius;
			set => m_radius = value;
		}

		/// <summary>
		/// 起始角(弧度)
		/// </summary>
		[Angle]
		[SerializeField]
		[Tooltip("起始角(°)")]
		private float m_startAngle;
		public float StartAngle
		{
			get => m_startAngle;
			set => m_startAngle = value;
		}

		/// <summary>
		/// 扫掠角(弧度)
		/// </summary>
		[Angle]
		[SerializeField]
		[Tooltip("扫掠角(°)")]
		[Range(-360, 360)]
		private float m_sweepAngle;
		public float SweepAngle
		{
			get => m_sweepAngle;
			set => m_sweepAngle = value;
		}

		/// <summary>
		/// 起始方向
		/// </summary>
		public Vector3 StartDirection => new Vector3(Mathf.Cos(m_startAngle), 0, Mathf.Sin(m_startAngle));
		public Vector3 WorldStartDirection => transform.TransformDirection(StartDirection);
		
		/// <summary>
		/// 终点角度(弧度)
		/// </summary>
		public float EndAngle => m_startAngle + m_sweepAngle;

		public override CurveType Type => CurveType.Arc;

		public Arc()
			: base()
		{
			width = 1.5f;
			color = Color.yellow;
		}

		public override Vector3 StartPoint()
		{
			return CalculatePoint(m_startAngle);
		}

		public override Vector3 EndPoint()
		{
			return CalculatePoint(m_startAngle + m_sweepAngle);
		}

		public override float GetLength()
		{
			//弧长
			return Mathf.Abs(m_sweepAngle) * m_radius;
		}

		public override Vector3 GetPoint(float t)
		{
			//归一化
			t = Mathf.Clamp01(t);
			return CalculatePoint(m_startAngle + t * m_sweepAngle);
		}

		public override IReadOnlyList<Vector3> GetPoints(float spacing = -1)
		{
			List<Vector3> result = new List<Vector3>();
			float length = GetLength();
			int sampleCount = (spacing == -1) ? 16 : Mathf.Max(1, Mathf.CeilToInt(length / spacing));
			for	(int i = 0; i <= sampleCount; i++)
			{
				float t = i / (float)sampleCount;
				result.Add(GetPoint(t));
			}
			return result;
		}

		/// <summary>
		/// 三点法计算圆弧
		/// </summary>
		/// <param name="center">圆心(世界坐标)</param>
		/// <param name="start">起始点(世界坐标)</param>
		/// <param name="end">终止点(世界坐标)</param>
		/// <param name="plane">目标平面</param>
		public void SetFromPoints(Vector3 center, Vector3 start, Vector3 end)
		{
			transform.position = center;
			m_centerPoint = Vector3.zero;
			
			Vector3 startPt = start - center;
			Vector3 endPt = end - center;

			Vector3 startDir = startPt.normalized;
			Vector3 endDir = endPt.normalized;

			m_radius = Vector3.Distance(m_centerPoint, startPt);	
			m_startAngle = Mathf.Atan2(startDir.z, startDir.x);	
			m_sweepAngle = -Vector3.SignedAngle(startDir, endDir, Vector3.up) * Mathf.Deg2Rad;

			Debug.Log("起始角：" + m_startAngle * Mathf.Rad2Deg);
			Debug.Log("扫掠角：" + m_sweepAngle * Mathf.Rad2Deg);
			Debug.Log(Vector3.SignedAngle(Vector3.right, Vector3.forward, Vector3.up));
		}

		#region IPickable Interface
		public float HitObject(Vector3 hitPt)
		{
			Vector3 localPt = transform.InverseTransformPoint(hitPt);
			localPt.y = 0.0f;

			Vector3 dir = localPt - m_centerPoint;
			float distance = dir.magnitude;
			if (distance < Mathf.Epsilon)
				return float.MaxValue;

			float angle = Mathf.Atan2(dir.z, dir.x);
			float delta = Mathf.DeltaAngle(m_startAngle * Mathf.Rad2Deg, angle * Mathf.Rad2Deg) * Mathf.Deg2Rad;

			if (m_sweepAngle >= 0)
			{
				if (delta < 0 || delta > m_sweepAngle)
					return float.MaxValue;
			}
			else
			{
				if (delta > 0 || delta < m_sweepAngle)
					return float.MaxValue;
			}
    		return Mathf.Abs(distance - m_radius);
		}

		public GameObject GetGameObject()
		{
			return this.gameObject;
		}
		#endregion
		
		private void OnDestroy()
		{
			PickManager.Unregister(this.gameObject);
		}

		private void OnDrawGizmos()
		{
			Handles.color = color;
			Vector3 center = transform.TransformPoint(m_centerPoint);
			Vector3 from = transform.TransformDirection(new Vector3(Mathf.Cos(m_startAngle), 0.0f, Mathf.Sin(m_startAngle)));
			float sweepAngle = Mathf.Abs(m_sweepAngle) * Mathf.Rad2Deg;
			Handles.DrawWireArc(center, transform.up, from, sweepAngle, m_radius);
		}

		private Vector3 CalculatePoint(float angle)
		{
			return m_centerPoint + new Vector3(Mathf.Cos(angle) * m_radius, 0.0f, Mathf.Sin(angle) * m_radius);
		}
	}
}
