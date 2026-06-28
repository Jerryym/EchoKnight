using UnityEngine;

namespace Echo
{
	public class AngleAttribute : PropertyAttribute
	{
		public readonly float MinAngle;
    	public readonly float MaxAngle;

		public AngleAttribute(float min = -360f, float max = 360f)
		{
			MinAngle = min;
			MaxAngle = max;
		}
	}
}
