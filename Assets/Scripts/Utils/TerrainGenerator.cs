using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

/// <summary>
/// 地形生成器
/// </summary>
public static class TerrainGenerator
{
	/// <summary>
	/// 创建地形Mesh
	/// </summary>
	/// <param name="setting">地形配置</param>
	/// <returns></returns>
	public static GameObject Generate(TerrainSetting setting, Material terrainMat)
	{
		if (setting == null)
		{
			Debug.LogError("TerrainSetting为空，无法生成地形！");
			return null;
		}

		//创建GameObject
		GameObject terrainGO = new GameObject(setting.name);

		//创建高度图
		float offsetX = Random.Range(0f, 10000f);
		float offsetY = Random.Range(0f, 10000f);
		float[,] heightMap = Noise.OctavePerlinNoiseMap(setting.terrainWidth + 1, setting.terrainLength + 1, setting.noiseScale,
			offsetX, offsetY,
			setting.minHeight, setting.maxHeight, setting.heightCurve, setting.octaves, setting.persistence, setting.lacunarity);

		//创建分块Mesh
		int chunkCountX = setting.terrainWidth / setting.chunkSize;
		int chunkCountZ = setting.terrainLength / setting.chunkSize;
		string savePath = $"Assets/Terrains/Meshes/{terrainGO.name}";
		for (int cz = 0; cz < chunkCountZ; cz++)
		{
			for (int cx = 0; cx < chunkCountX; cx++)
			{
				GameObject chunkGO = new GameObject($"Chunk_{cx}_{cz}");
				chunkGO.transform.parent = terrainGO.transform;
				chunkGO.transform.localPosition = new Vector3(cx * setting.chunkSize, 0, cz * setting.chunkSize);
				
				//添加组件
				MeshFilter meshFilter = chunkGO.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = chunkGO.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = terrainMat;

				//创建Mesh
				Mesh mesh = CreateMesh(setting, cx, cz, heightMap);
				mesh.name = $"Chunk_{cx}_{cz}";
				meshFilter.sharedMesh = mesh;

				//保存Mesh
				SaveMesh(savePath, mesh);
			}
		}
		return terrainGO;
	}

	private static Mesh CreateMesh(TerrainSetting setting, int chunkX, int chunkZ, float[,] heightMap)
	{
		int heightMapXIndex = chunkX * setting.chunkSize;
		int heightMapZIndex = chunkZ * setting.chunkSize;

		//创建顶点数组
		Vector3[] vertices = new Vector3[(setting.chunkSize + 1) * (setting.chunkSize + 1)];
		int index = 0;
		for (int z = 0; z <= setting.chunkSize; z++)
		{
			for (int x = 0; x <= setting.chunkSize; x++)
			{
				float y = heightMap[heightMapXIndex + x, heightMapZIndex + z];
				vertices[index] = new Vector3(x, y, z);
				index++;
			}
		}
		Debug.Log("顶点数量：" + vertices.Length);

		//创建索引数组
		int[] triangles = new int[setting.chunkSize * setting.chunkSize * 6];
		int triIndex = 0, verIndex = 0;
		for (int z = 0; z < setting.chunkSize; z++)
		{
			for (int x = 0; x < setting.chunkSize; x++)
			{
				//第一个三角形
				triangles[triIndex + 0] = verIndex + 0;
				triangles[triIndex + 1] = verIndex + setting.chunkSize + 1;
				triangles[triIndex + 2] = verIndex + 1;

				//第二个三角形
				triangles[triIndex + 3] = verIndex + 1;
				triangles[triIndex + 4] = verIndex + setting.chunkSize + 1;
				triangles[triIndex + 5] = verIndex + setting.chunkSize + 2;

				verIndex++;
				triIndex += 6;
			}
			verIndex++;
		}
		Debug.Log("索引数量：" + triangles.Length);

		//创建UV
		index = 0;
		Vector2[] uvs = new Vector2[vertices.Length];
		for (int z = 0; z <= setting.chunkSize; z++)
		{
			for (int x = 0; x <= setting.chunkSize; x++)
			{
				uvs[index] = new Vector2(x / (float)setting.chunkSize, z / (float)setting.chunkSize);
				index++;
			}
		}

		//创建Mesh
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		return mesh;
	}

	private static void SaveMesh(string savePath, Mesh mesh)
	{
		if (mesh == null)
			return;

		if (!Directory.Exists(savePath))
			Directory.CreateDirectory(savePath);

		string fileName = savePath + $"/{mesh.name}.asset";
		AssetDatabase.CreateAsset(mesh, fileName);
		AssetDatabase.SaveAssets();
	}
}
