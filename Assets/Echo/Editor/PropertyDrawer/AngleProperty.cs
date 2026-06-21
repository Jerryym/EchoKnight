using UnityEditor;
using UnityEngine;

namespace Echo.Editor
{
	[CustomPropertyDrawer(typeof(AngleAttribute))]
	public class AngleProperty : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Float)
			{
				EditorGUI.LabelField(position, label.text, "Angle only supports float");
				return;
			}

			// 弧度 -> 角度
			float degrees = property.floatValue * Mathf.Rad2Deg;

			EditorGUI.BeginChangeCheck();
			degrees = EditorGUI.FloatField(position, label, degrees);

			// 角度 -> 弧度
			if (EditorGUI.EndChangeCheck())
			{
				property.floatValue = degrees * Mathf.Deg2Rad;
			}
		}
	}
}
