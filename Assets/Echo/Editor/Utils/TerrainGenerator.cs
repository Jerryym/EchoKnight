using System.Collections.Generic;
using UnityEngine;

namespace Echo.Editor.Utils
{
	/// <summary>
	/// 地形生成器
	/// </summary>
	public static class TerrainGenerator
	{
		/// <summary>
		/// 创建地形Mesh
		/// </summary>
		public static Dictionary<Vector3, Mesh> GenerateTerrain(TerrainSetting setting)
		{
			if (setting == null)
			{
				Debug.LogError("TerrainSetting为空，无法生成地形！");
				return null;
			}

			//创建高度图
			float[,] heightMap = GenerateHeightMap(setting);

			//创建分块Mesh
			Dictionary<Vector3, Mesh> chunkMeshMap = new Dictionary<Vector3, Mesh>();
			int chunkCountX = setting.terrainWidth / setting.chunkSize;
			int chunkCountZ = setting.terrainLength / setting.chunkSize;
			for (int cz = 0; cz < chunkCountZ; cz++)
			{
				for (int cx = 0; cx < chunkCountX; cx++)
				{
					//创建Mesh
					Mesh mesh = CreateMesh(setting, cx, cz, heightMap, 1);
					mesh.name = $"Chunk_{cx}_{cz}";

					Vector3 position = new Vector3(cx * setting.chunkSize, 0, cz * setting.chunkSize);
					chunkMeshMap.Add(position, mesh);
				}
			}
			return chunkMeshMap;
		}

		/// <summary>
		/// 创建地形LOD分块Mesh
		/// </summary>
		public static Dictionary<int, Dictionary<Vector3, Mesh>> GenerateTerrainLODs(TerrainSetting setting)
		{
			if (setting == null)
			{
				Debug.LogError("TerrainSetting为空，无法生成地形！");
				return null;
			}

			//创建高度图
			float[,] heightMap = GenerateHeightMap(setting);

			//创建分块Mesh
			Dictionary<int, Dictionary<Vector3, Mesh>> lodMeshMap = new Dictionary<int, Dictionary<Vector3, Mesh>>();
			int chunkCountX = setting.terrainWidth / setting.chunkSize;
			int chunkCountZ = setting.terrainLength / setting.chunkSize;
			for (int i = 0; i < setting.lodLevel; ++i)
			{
				Dictionary<Vector3, Mesh> chunkMeshMap = new Dictionary<Vector3, Mesh>(chunkCountX * chunkCountZ);
				for (int cz = 0; cz < chunkCountZ; cz++)
				{
					for (int cx = 0; cx < chunkCountX; cx++)
					{
						//创建Mesh
						Mesh mesh = CreateMesh(setting, cx, cz, heightMap, 1 << i);
						mesh.name = $"Chunk_{cx}_{cz}";
						Vector3 position = new Vector3(cx * setting.chunkSize, 0, cz * setting.chunkSize);
						chunkMeshMap.Add(position, mesh);
					}
				}
				lodMeshMap.Add(i, chunkMeshMap);
			}
			return lodMeshMap;
		}

		/// <summary>
		/// 合并分块Mesh
		/// </summary>
		public static Mesh CombineChunkMeshes(MeshFilter[] meshFilters, Transform transform, string meshName)
		{
			if (meshFilters == null || meshFilters.Length == 0 || transform == null)
				return null;

			//获取子对象的MeshFilter
			List<CombineInstance> combineInstances = new List<CombineInstance>();
			foreach (MeshFilter meshFilter in meshFilters)
			{
				if (meshFilter == null || meshFilter.sharedMesh == null)
					continue;

				CombineInstance combineInstance = new CombineInstance
				{
					mesh = meshFilter.sharedMesh,
					transform = meshFilter.transform.localToWorldMatrix * transform.worldToLocalMatrix
				};
				combineInstances.Add(combineInstance);
			}

			if (combineInstances.Count == 0)
				return null;

			//创建Mesh
			Mesh combinedMesh = new Mesh()
			{
				name = meshName,
				indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
			};
			combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
			combinedMesh.RecalculateBounds();
			combinedMesh.RecalculateNormals();
			combinedMesh.RecalculateTangents();

			return combinedMesh;
		}

		/// <summary>
		/// 生成高度图
		/// </summary>
		/// <returns></returns>
		private static float[,] GenerateHeightMap(TerrainSetting setting)
		{
			float offsetX = Random.Range(0f, 10000f);
			float offsetY = Random.Range(0f, 10000f);
			float[,] heightMap = Noise.OctavePerlinNoiseMap(setting.terrainWidth + 1, setting.terrainLength + 1, setting.noiseScale,
				offsetX, offsetY,
				setting.minHeight, setting.maxHeight, setting.heightCurve, setting.octaves, setting.persistence, setting.lacunarity);

			return heightMap;
		}

		private static Mesh CreateMesh(TerrainSetting setting, int chunkX, int chunkZ, float[,] heightMap, int step)
		{
			int heightMapXIndex = chunkX * setting.chunkSize;
			int heightMapZIndex = chunkZ * setting.chunkSize;

			int lodSize = setting.chunkSize / step;
			int verticesCount = lodSize + 1;

			//创建顶点数组
			Vector3[] vertices = new Vector3[verticesCount * verticesCount];
			Vector2[] uvs = new Vector2[vertices.Length];

			int index = 0;
			for (int z = 0; z <= setting.chunkSize; z += step)
			{
				for (int x = 0; x <= setting.chunkSize; x += step)
				{
					float y = heightMap[heightMapXIndex + x, heightMapZIndex + z];
					vertices[index] = new Vector3(x, y, z);
					uvs[index] = new Vector2(x / (float)setting.chunkSize, z / (float)setting.chunkSize);
					index++;
				}
			}
			Debug.Log("顶点数量：" + vertices.Length);

			//创建索引数组
			int[] triangles = new int[lodSize * lodSize * 6];
			int triIndex = 0, verIndex = 0;
			for (int z = 0; z < lodSize; z++)
			{
				for (int x = 0; x < lodSize; x++)
				{
					//第一个三角形
					triangles[triIndex + 0] = verIndex + 0;
					triangles[triIndex + 1] = verIndex + verticesCount;
					triangles[triIndex + 2] = verIndex + 1;

					//第二个三角形
					triangles[triIndex + 3] = verIndex + 1;
					triangles[triIndex + 4] = verIndex + verticesCount;
					triangles[triIndex + 5] = verIndex + verticesCount + 1;

					verIndex++;
					triIndex += 6;
				}
				verIndex++;
			}
			Debug.Log("索引数量：" + triangles.Length);

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
}

