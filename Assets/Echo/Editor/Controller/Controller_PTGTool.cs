using Echo.Editor.UI;
using Echo.Editor.Utils;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor
{
	public class Controller_PTGTool : IDisposable
	{
		private View_PTGTool m_view = null;
		private Model_PTGTool m_viewModel = null;

		private Dictionary<string, GameObject> m_previewGOMap = null;
		
		/// <summary>
		/// 资源销毁标识符
		/// </summary>
		private bool m_isDisposed =  false;

		public Controller_PTGTool(View_PTGTool view)
		{
			var editorSettings = EchoEditorSettings.instance;
			m_view = view;
			m_view.InitData(editorSettings.model_PTGTool);//初始化数据

			m_previewGOMap = new Dictionary<string, GameObject>();

			//订阅事件
			m_view.RefreshClicked += OnRefreshClick;
			m_view.SaveSettingClicked += OnSaveSettingClick;
			m_view.OkClicked += OnOkClick;
			m_view.CancelClicked += OnCancelClick;
		}

		public void Dispose()
		{
			if (m_isDisposed)
				return;
			Debug.Log("PTGToolController Dispose");

			//事件解绑
			m_view.RefreshClicked -= OnRefreshClick;
			m_view.SaveSettingClicked -= OnSaveSettingClick;
			m_view.OkClicked -= OnOkClick;
			m_view.CancelClicked -= OnCancelClick;

			//删除预览
			ClearPreview();

			m_isDisposed = true;
		}

		private void OnRefreshClick()
		{
			m_viewModel = m_view.GetData();
			if (string.IsNullOrWhiteSpace(m_viewModel.terrainName))
			{
				RPGEditorToolWindow.ShowTip("地形名称不可为空!", StatusBar.TipLevel.Warning);
				return;
			}
			TerrainSetting terrainSetting = CreateSetting();

			//删除预览
			ClearPreview();
			//创建预览
			CreatePreview(terrainSetting);
		}

		private void OnSaveSettingClick()
		{
			m_viewModel = m_view.GetData();
			if (string.IsNullOrWhiteSpace(m_viewModel.configSavePath))
			{
				RPGEditorToolWindow.ShowTip("地形配置文件路径为空, 无法保存配置!", StatusBar.TipLevel.Warning);
				return;
			}

			//保存TerrainSetting
			TerrainSetting terrainSetting = CreateSetting();
			SaveTerrainSetting(terrainSetting);
		}

		private void OnOkClick()
		{
			if (m_previewGOMap.Count == 0)
				return;

			m_viewModel = m_view.GetData();
			if (string.IsNullOrWhiteSpace(m_viewModel.terrainSavePath))
			{
				RPGEditorToolWindow.ShowTip("地形输出路径为空!", StatusBar.TipLevel.Warning);
				return;
			}

			if (string.IsNullOrWhiteSpace(m_viewModel.terrainName))
			{
				RPGEditorToolWindow.ShowTip("地形名称不可为空!", StatusBar.TipLevel.Warning);
				return;
			}

			TerrainSetting terrainSetting = CreateSetting();
			List<Mesh> meshes = CombineMeshes();
			if (meshes == null || meshes.Count == 0)
				return;

			using (new EditorUndoScope("Generate Terrain"))
			{
				//保存Mesh
				SaveMesh(meshes);
				//创建LODGroup
				GameObject terrainGO;
				if (terrainSetting.enableLOD)
				{
					terrainGO = CreateLODGroup(meshes, terrainSetting.lodLevel, terrainSetting.material);
					terrainGO.name = terrainSetting.terrainName;

					//保存prefab
					SavePrefab(terrainGO);
				}
				else
				{
					terrainGO = new GameObject(terrainSetting.terrainName);

					//添加组件
					MeshFilter meshFilter = terrainGO.AddComponent<MeshFilter>();
					meshFilter.sharedMesh = meshes[0];
					MeshRenderer meshRenderer = terrainGO.AddComponent<MeshRenderer>();
					meshRenderer.sharedMaterial = terrainSetting.material;
					MeshCollider meshCollider = terrainGO.AddComponent<MeshCollider>();
					meshCollider.sharedMesh = meshes[0];

					//保存prefab
					SavePrefab(terrainGO);
				}
			}
			//删除预览
			ClearPreview();

			//保存界面数据
			var editorSettings = EchoEditorSettings.instance;
			editorSettings.model_PTGTool = m_viewModel;
			editorSettings.SaveSettings();
		}

		private List<Mesh> CombineMeshes()
		{
			List<Mesh> meshes = new List<Mesh>();
			foreach (var kvp in m_previewGOMap)
			{
				if (kvp.Value == null)
					continue;

				string name = kvp.Key;
				GameObject previewGO = kvp.Value;
				MeshFilter[] meshFilters = previewGO.GetComponentsInChildren<MeshFilter>();
				if (meshFilters == null)
					continue;

				Mesh mesh = TerrainGenerator.CombineChunkMeshes(meshFilters, previewGO.transform, name);
				if (mesh != null)
				{
					meshes.Add(mesh);
				}
			}
			return meshes;
		}

		private void OnCancelClick()
		{
			//删除预览
			ClearPreview();
		}

		/// <summary>
		/// 保存mesh
		/// </summary>
		private void SaveMesh(List<Mesh> meshes)
		{
			string meshPath = $"{m_viewModel.terrainSavePath}/{m_viewModel.terrainName}/Meshes";
			meshPath = FileUtil.GetPhysicalPath(meshPath);//转换成物理地址
			if (!FileUtility.DirectoryExists(meshPath))
			{
				FileUtility.CreateDirectory(meshPath);
				AssetDatabase.Refresh();
			}
			meshPath = FileUtil.GetLogicalPath(meshPath);//转成逻辑地址

			foreach (var mesh in meshes)
			{
				if (mesh == null)
					continue;
				
				string meshName = mesh.name;
				string assetFileName = $"{meshPath}/{meshName}.asset";
				string assetPath = AssetDatabase.GenerateUniqueAssetPath(assetFileName);
				AssetDatabase.CreateAsset(mesh, assetPath);

				Debug.Log($"保存{meshName}Mesh成功!");
			}
		}

		/// <summary>
		/// 创建LODGroup
		/// </summary>
		private GameObject CreateLODGroup(List<Mesh> lodMeshes, int lodLevel, Material material)
		{
			GameObject terrainGO = new GameObject();
			var lodPercent = GetLODPercents(lodLevel);

			//添加LODGroup
			var lodGroup = terrainGO.AddComponent<LODGroup>();
			List<LOD> lods = new List<LOD>();
			for (int i = 0; i < lodMeshes.Count; i++)
			{
				var mesh = lodMeshes[i];
				GameObject lodGO = new GameObject(mesh.name);
				lodGO.transform.SetParent(terrainGO.transform);

				//添加组件
				MeshFilter meshFilter = lodGO.AddComponent<MeshFilter>();
				meshFilter.sharedMesh = mesh;
				MeshRenderer meshRenderer = lodGO.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = material;
				MeshCollider meshCollider = lodGO.AddComponent<MeshCollider>();
				meshCollider.sharedMesh = mesh;

				lods.Add(new LOD(lodPercent[i], new Renderer[] { meshRenderer }));
			}
			lodGroup.SetLODs(lods.ToArray());
			lodGroup.RecalculateBounds();

			return terrainGO;
		}

		private void SavePrefab(GameObject terrainGO)
		{
			string savePath = $"{m_viewModel.terrainSavePath}/{m_viewModel.terrainName}";
			savePath = FileUtil.GetPhysicalPath(savePath);//转换成物理地址
			if (!FileUtility.DirectoryExists(savePath))
			{
				FileUtility.CreateDirectory(savePath);
				AssetDatabase.Refresh();
			}
			savePath = FileUtil.GetLogicalPath(savePath);//转成逻辑地址

			//保存prefab
			string prefabPath = $"{savePath}/{terrainGO.name}.prefab";
			prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
			GameObject prefab = PrefabUtility.SaveAsPrefabAsset(terrainGO, prefabPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			RPGEditorToolWindow.ShowTip($"保存{terrainGO.name}成功!");

			//加载prefab
			GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			EditorUndoUtility.RegisterCreateObject(instance, "Generate Terrain");

			//删除terrainGO
			UnityEngine.Object.DestroyImmediate(terrainGO);
		}

		private void SaveTerrainSetting(TerrainSetting terrainSetting)
		{
			if (m_viewModel.terrainSetting == null)
			{
				string path = $"{m_viewModel.configSavePath}/{terrainSetting.terrainName}.asset";
				AssetDatabase.CreateAsset(terrainSetting, path);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				RPGEditorToolWindow.ShowTip($"保存{terrainSetting.name}配置成功!");
			}
			else
			{
				terrainSetting = m_viewModel.terrainSetting;
				EditorUndoUtility.RecordObject(terrainSetting, "Update Terrain Setting");
				
				UpdateSetting(terrainSetting);
				EditorUtility.SetDirty(terrainSetting);
				AssetDatabase.SaveAssets();

				RPGEditorToolWindow.ShowTip($"更新{terrainSetting.name}配置成功!");
			}
		}

		/// <summary>
		/// 创建TerrainSetting
		/// </summary>
		private TerrainSetting CreateSetting()
		{
			if (m_viewModel == null)
				return null;

			TerrainSetting terrainSetting = ScriptableObject.CreateInstance<TerrainSetting>();
			terrainSetting.terrainName = m_viewModel.terrainName;
			terrainSetting.material = m_viewModel.terrainMaterial;

			//地形参数
			terrainSetting.terrainWidth = m_viewModel.terrainWidth;
			terrainSetting.terrainLength = m_viewModel.terrainLength;
			terrainSetting.chunkSize = m_viewModel.chunkSize;
			terrainSetting.minHeight = m_viewModel.minHeight;
			terrainSetting.maxHeight = m_viewModel.maxHeight;
			terrainSetting.heightCurve = m_viewModel.heightCurve;

			//噪声参数
			terrainSetting.noiseScale = m_viewModel.noiseScale;
			terrainSetting.octaves = m_viewModel.octaves;
			terrainSetting.persistence = m_viewModel.persistence;
			terrainSetting.lacunarity = m_viewModel.lacunarity;

			//LOD
			terrainSetting.enableLOD = m_viewModel.enableLOD;
			terrainSetting.lodLevel = m_viewModel.lodLevel + 1;

			return terrainSetting;
		}

		/// <summary>
		/// 更新TerrainSetting
		/// </summary>
		private void UpdateSetting(TerrainSetting terrainSetting)
		{
			terrainSetting.terrainName = m_viewModel.terrainName;
			terrainSetting.material = m_viewModel.terrainMaterial;

			//地形参数
			terrainSetting.terrainWidth = m_viewModel.terrainWidth;
			terrainSetting.terrainLength = m_viewModel.terrainLength;
			terrainSetting.chunkSize = m_viewModel.chunkSize;
			terrainSetting.minHeight = m_viewModel.minHeight;
			terrainSetting.maxHeight = m_viewModel.maxHeight;
			terrainSetting.heightCurve = m_viewModel.heightCurve;

			//噪声参数
			terrainSetting.noiseScale = m_viewModel.noiseScale;
			terrainSetting.octaves = m_viewModel.octaves;
			terrainSetting.persistence = m_viewModel.persistence;
			terrainSetting.lacunarity = m_viewModel.lacunarity;

			//LOD
			terrainSetting.enableLOD = m_viewModel.enableLOD;
			terrainSetting.lodLevel = m_viewModel.lodLevel + 1;
		}

		/// <summary>
		/// 创建预览
		/// </summary>
		private void CreatePreview(TerrainSetting terrainSetting)
		{
			if (!terrainSetting.enableLOD)//不开启LOD
			{
				Dictionary<Vector3, Mesh> terrainMeshMap = TerrainGenerator.GenerateTerrain(terrainSetting);
				if (terrainMeshMap.Count == 0)
					return;

				GameObject terrainGO = new GameObject(terrainSetting.terrainName);
				foreach (var chunkMesh in terrainMeshMap)
				{
					if (chunkMesh.Value == null)
						continue;

					Vector3 position = chunkMesh.Key;
					Mesh mesh = chunkMesh.Value;
					CreateChunkMeshGO(position, mesh, terrainSetting.material, terrainGO.transform);
				}
				terrainGO.hideFlags = HideFlags.DontSaveInEditor;
				m_previewGOMap[terrainGO.name] = terrainGO;
			}
			else
			{
				Dictionary<int, Dictionary<Vector3, Mesh>> lodMeshMap = TerrainGenerator.GenerateTerrainLODs(terrainSetting);
				if (lodMeshMap.Count == 0)
					return;

				foreach (var lodChunkMeshMap in lodMeshMap)
				{
					if (lodChunkMeshMap.Value == null)
						continue;

					GameObject terrainGO = new GameObject($"{terrainSetting.terrainName}_LOD{lodChunkMeshMap.Key}");
					terrainGO.transform.position = new Vector3((terrainSetting.terrainWidth + 20) * lodChunkMeshMap.Key, 0, 0);
					foreach (var chunkMesh in lodChunkMeshMap.Value)
					{
						if (chunkMesh.Value == null)
							continue;

						Vector3 position = chunkMesh.Key;
						Mesh mesh = chunkMesh.Value;
						CreateChunkMeshGO(position, mesh, terrainSetting.material, terrainGO.transform);
					}
					terrainGO.hideFlags = HideFlags.DontSaveInEditor;
					m_previewGOMap[terrainGO.name] = terrainGO;
				}
			}
		}

		/// <summary>
		/// 清空预览
		/// </summary>
		private void ClearPreview()
		{
			foreach (var kvp in m_previewGOMap)
			{
				if (kvp.Value != null)
				{
					UnityEngine.Object.DestroyImmediate(kvp.Value);
				}
			}
			m_previewGOMap.Clear();
		}

		private void CreateChunkMeshGO(Vector3 position, Mesh mesh, Material material, Transform parent)
		{
			GameObject chunkGO = new GameObject(mesh.name);
			chunkGO.transform.parent = parent;
			chunkGO.transform.localPosition = position;

			//添加组件
			MeshFilter meshFilter = chunkGO.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = mesh;
			MeshRenderer meshRenderer = chunkGO.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = material;
			MeshCollider meshCollider = chunkGO.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = mesh;
		}

		private float[] GetLODPercents(int lodLevel)
		{
			float start = 0.6f;
			float factor = 0.5f;

			float[] result = new float[lodLevel];
			float current = start;
			for (int i = 0; i < lodLevel; i++)
			{
				result[i] = current;
				current *= factor;
			}

			return result;
		}
	}
}
