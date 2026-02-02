using System.Collections.Generic;
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
	public static GameObject Generate(string sTerrainName, TerrainSetting setting, Material terrainMat)
	{
		if (setting == null)
		{
			Debug.LogError("TerrainSetting为空，无法生成地形！");
			return null;
		}

		if (sTerrainName == null)
		{
			sTerrainName = "Terrain";
		}
		
		//创建高度图
		float offsetX = Random.Range(0f, 10000f);
		float offsetY = Random.Range(0f, 10000f);
		float[,] heightMap = Noise.OctavePerlinNoiseMap(setting.terrainWidth + 1, setting.terrainLength + 1, setting.noiseScale,
			offsetX, offsetY,
			setting.minHeight, setting.maxHeight, setting.heightCurve, setting.octaves, setting.persistence, setting.lacunarity);

		//创建GameObject
		GameObject terrainGO = new GameObject(sTerrainName);

		//创建分块Mesh
		int chunkCountX = setting.terrainWidth / setting.chunkSize;
		int chunkCountZ = setting.terrainLength / setting.chunkSize;
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
				MeshCollider meshCollider = chunkGO.AddComponent<MeshCollider>();

				//创建Mesh
				Mesh mesh = CreateMesh(setting, cx, cz, heightMap);
				mesh.name = $"Chunk_{cx}_{cz}";
				meshFilter.sharedMesh = mesh;
				meshRenderer.sharedMaterial = terrainMat;
				meshCollider.sharedMesh = mesh;
			}
		}
		return terrainGO;
	}

	/// <summary>
	/// 合并分块Mesh
	/// </summary>
	/// <param name="terrainGO"></param>
	public static GameObject CombineChunkMeshes(GameObject terrainGO, Material terrainMat)
	{
		if (terrainGO == null)
			return null;

		//获取子对象的MeshFilter
		MeshFilter[] meshFilters = terrainGO.GetComponentsInChildren<MeshFilter>();
		List<CombineInstance> combineInstances = new List<CombineInstance>();
		foreach (MeshFilter meshFilter in meshFilters)
		{
			if (meshFilter == null || meshFilter.sharedMesh == null)
				continue;
		
			CombineInstance combineInstance = new CombineInstance();
			combineInstance.mesh = meshFilter.sharedMesh;
			combineInstance.transform = meshFilter.transform.localToWorldMatrix;
			combineInstances.Add(combineInstance);
		}

		GameObject newTerrainGO = new GameObject(terrainGO.name);
		MeshFilter newMeshFilter = newTerrainGO.AddComponent<MeshFilter>();
		MeshRenderer newMeshRenderer = newTerrainGO.AddComponent<MeshRenderer>();
		MeshCollider meshCollider = newTerrainGO.AddComponent<MeshCollider>();

		//创建Mesh
		Mesh combinedMesh = new Mesh();
		combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
		combinedMesh.RecalculateBounds();
    	combinedMesh.RecalculateNormals();
		combinedMesh.RecalculateTangents();

		newMeshFilter.sharedMesh = combinedMesh;
		newMeshRenderer.sharedMaterial = terrainMat;
		meshCollider.sharedMesh = combinedMesh;

		return newTerrainGO;
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
		
		//重新计算法线
		mesh.RecalculateNormals();
		
		return mesh;
	}

}
