using System;
using UnityEngine;

namespace Echo.Editor
{
	[Serializable]
	public class Model_PTGTool
	{
		public TerrainSetting terrainSetting = null;
		public string configSavePath;
		public string terrainSavePath;

		public string terrainName = "Terrain";
		public TerrainType terrainType = TerrainType.Plain;
		public Material terrainMaterial;

		public int terrainWidth = 256;
		public int terrainLength = 256;
		public int chunkSize = 32;
		public float minHeight = 0f;
		public float maxHeight = 20f;
		public AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 1, 1);

		public float noiseScale = 100f;
		public int octaves = 4;
		public float persistence = 0.4f;
		public float lacunarity = 1.8f;

		public bool enableLOD = true;
		public int lodLevel = 0;
	}
}
