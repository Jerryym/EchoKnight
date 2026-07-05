using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Component
{
	public class Circle : Curve, IPickable
	{
		/// <summary>
		/// 圆心
		/// </summary>
		[SerializeField]
		private Vector3 m_center;
		public Vector3 CenterPoint
		{
			get => m_center;
			set => m_center = value;
		}
		public Vector3 WorldCenterPoint => transform.TransformPoint(m_center);

		/// <summary>
		/// 半径
		/// </summary>
		[SerializeField]
		private float m_radius;
		public float Radius
		{
			get => m_radius;
			set => m_radius = value;
		}


		public override CurveType Type => CurveType.Circle;

		public Circle()
			: base()
		{
			width = 1.5f;
			color = Color.yellow;
		}

		public override Vector3 StartPoint()
		{
			return m_center + Vector3.right * m_radius;
		}

		public override Vector3 EndPoint()
		{
			return m_center + Vector3.right * m_radius;
		}

		public GameObject GetGameObject()
		{
			return this.gameObject;
		}

		public override float GetLength()
		{
			return 2 * Mathf.PI * m_radius;
		}

		public override Vector3 GetPoint(float t)
		{
			t = Mathf.Repeat(t, 1.0f);
			float angle = t * Mathf.PI * 2.0f;
			return m_center + new Vector3(Mathf.Cos(angle) * m_radius, 0, Mathf.Sin(angle) * m_radius);
		}

		public override IReadOnlyList<Vector3> GetPoints(float spacing = -1)
		{
			List<Vector3> result = new List<Vector3>();
			float circumference = GetLength();
			if (spacing <= 0)
			{
				const int segment = 64;
				for (int i = 0; i <= segment; i++)
				{
					float t = i / (float)segment;
					result.Add(GetPoint(t));
				}
			}
			else
			{
				int segment = Mathf.Max(3, Mathf.CeilToInt(circumference / spacing));
				for (int i = 0; i <= segment; i++)
				{
					float t = i / (float)segment;
					result.Add(GetPoint(t));
				}
			}
			return result;
		}

		public float HitObject(Vector3 hitPt)
		{
			float distToCenter = Vector3.Distance(hitPt, WorldCenterPoint);
			return Mathf.Abs(distToCenter - m_radius);
		}

		private void OnDestroy()
		{
			PickManager.Unregister(this.gameObject);
		}

		private void OnDrawGizmos()
		{
			Vector3 centerPt = transform.TransformPoint(CenterPoint);

			Handles.color = color;
			Handles.DrawWireDisc(centerPt, Vector3.up, m_radius);
		}
	}
}
