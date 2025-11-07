using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		//绘制默认的属性
		DrawDefaultInspector();
		
		//创建地形生成器
		TerrainGenerator terrainGenerator = (TerrainGenerator)target;
		
		//创建按钮
		GUILayout.Space(10);
		if (GUILayout.Button("生成"))
		{
			terrainGenerator.GenerateTerrain();
		}
    }
}
