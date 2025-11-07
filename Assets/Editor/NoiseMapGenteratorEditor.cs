using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseMapGenerator))]
public class NoiseMapGenteratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		NoiseMapGenerator generator = (NoiseMapGenerator)target;
		GUILayout.Space(10);
		if (GUILayout.Button("生成"))
		{
			generator.GenerateNoiseMap();
		}
	}
}
