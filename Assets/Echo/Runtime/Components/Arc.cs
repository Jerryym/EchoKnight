using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Command
{
	public class Arc : Curve, IPickable
	{
		[SerializeField]
		/// <summary>
		/// 圆心点
		/// </summary>
		private Vector3 m_centerPoint;
		public Vector3 CenterPoint => m_centerPoint;
		
		[SerializeField]
		/// <summary>
		/// 半径
		/// </summary>
		private float m_radius;
		public float Radius => m_radius;

		/// <summary>
		/// 起始角
		/// </summary>
		[Angle]
		[SerializeField]
		private float m_startAngle;
		public float StartAngle => m_startAngle;

		/// <summary>
		/// 扫掠角
		/// </summary>
		[Angle]
		[SerializeField]
		private float m_sweepAngle;
		public float SweepAngle => m_sweepAngle;

		public override CurveType Type => CurveType.Arc;
		public override float Width { get; set; } = 1.5f;
		public override Color Color { get; set; } = Color.yellow;
		
		public override Vector3 StartPoint()
		{
			throw new System.NotImplementedException();
		}

		public override Vector3 EndPoint()
		{
			throw new System.NotImplementedException();
		}

		public override float GetLength()
		{
			return Mathf.Abs(m_sweepAngle) * m_radius;
		}

		public override Vector3 GetPoint(float t)
		{
			throw new System.NotImplementedException();
		}

		public override IReadOnlyList<Vector3> GetPoints(float spacing = -1)
		{
			throw new System.NotImplementedException();
		}

		#region IPickable Interface
		public float HitObject(Vector2 hitPt)
		{
			throw new System.NotImplementedException();
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
			//转成世界坐标
			Vector3 centerPt = transform.TransformPoint(m_centerPoint);

			Handles.color = Color;
		}

		private Vector3 CalculatePoint(float angle)
		{
			return m_centerPoint + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * m_radius;
		}
	}

}
