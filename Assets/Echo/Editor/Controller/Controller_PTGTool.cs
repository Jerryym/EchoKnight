using Echo.Editor.UI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor
{
	public class Controller_PTGTool : IDisposable
	{
		private View_PTGTool m_view = null;
		
		private TerrainSetting m_terrainSetting = null;
		private List<GameObject> m_terrainGOList = null;
		
		/// <summary>
		/// 资源销毁标识符
		/// </summary>
		private bool m_isDisposed =  false;

		public Controller_PTGTool(View_PTGTool view)
		{
			m_view = view;
			m_view.InitData(PTGToolSettings.instance.viewData);//初始化数据

			m_terrainSetting = ScriptableObject.CreateInstance<TerrainSetting>();
			m_terrainGOList = new List<GameObject>();

			//订阅事件
			m_view.OnUpdateClicked += UpdateSetting;
			m_view.OnRefreshClicked += GenerateTerrain;
			m_view.OnSaveClicked += SaveTerrain;
		}

		public void Dispose()
		{
			if (m_isDisposed)
				return;
			Debug.Log("PTGToolController Dispose");

			//事件解绑
			m_view.OnUpdateClicked -= UpdateSetting;
			m_view.OnRefreshClicked -= GenerateTerrain;
			m_view.OnSaveClicked -= SaveTerrain;

			//删除预览
			CleanInvalidPreview();
			foreach (var item in m_terrainGOList)
			{
				if (item)
				{
					Undo.DestroyObjectImmediate(item);
				}
			}
			m_terrainGOList.Clear();

			m_isDisposed = true;
		}

		/// <summary>
		/// 更新地形配置
		/// </summary>
		private void UpdateSetting()
		{
			var param = m_view.GetData();
			if (param.terrainSetting != null)//更新配置
			{
				m_terrainSetting = param.terrainSetting;
				Undo.RecordObject(m_terrainSetting, "Update Terrain Setting");
				CreateSettingData(param);

				EditorUtility.SetDirty(m_terrainSetting);
				AssetDatabase.SaveAssets();

				RPGEditorToolWindow.ShowTip($"更新{m_terrainSetting.name}配置成功!");
			}
			else//保存配置
			{
				if (string.IsNullOrEmpty(m_view.ConfigSavePath))
				{
					RPGEditorToolWindow.ShowTip("地形配置文件路径为空!", StatusBar.TipLevel.Warning);
					return;
				}
				string path = $"{m_view.ConfigSavePath}/{param.terrainName}.asset";

				CreateSettingData(param);
				AssetDatabase.CreateAsset(m_terrainSetting, path);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				RPGEditorToolWindow.ShowTip($"保存{m_terrainSetting.name}配置成功!");
			}

			//保存界面数据
			PTGToolSettings.instance.viewData = param;
			PTGToolSettings.instance.Save();
		}

		/// <summary>
		/// 生成地形
		/// </summary>
		private void GenerateTerrain()
		{
			CleanInvalidPreview();
			if (m_terrainGOList.Count != 0)
			{
				foreach (var item in m_terrainGOList)
				{
					if (item)
					{
						Undo.DestroyObjectImmediate(item);
					}
				}
				m_terrainGOList.Clear();
			}

			var param = m_view.GetData();
			CreateSettingData(param);
			if (!m_terrainSetting.enableLOD)
			{
				var terrainGO = Utils.TerrainGenerator.GenerateTerrain(param.terrainName, m_terrainSetting);
				terrainGO.hideFlags = HideFlags.DontSave;
				m_terrainGOList.Add(terrainGO);
			}
			else
			{
				m_terrainGOList = Utils.TerrainGenerator.GenerateTerrainLODs(param.terrainName, m_terrainSetting);
				foreach (var item in m_terrainGOList)
				{
					if (item)
					{
						item.hideFlags = HideFlags.DontSave;
					}
				}
			}

			foreach (var item in m_terrainGOList)
			{
				Undo.RegisterCreatedObjectUndo(item, "Generate Terrain");
			}

			//保存界面数据
			PTGToolSettings.instance.viewData = param;
			PTGToolSettings.instance.Save();
		}

		/// <summary>
		/// 保存地形
		/// </summary>
		private void SaveTerrain()
		{
			CleanInvalidPreview();
			if (m_terrainGOList.Count == 0)
				return;
			
			if (string.IsNullOrEmpty(m_view.TerrainSavePath))
			{
				RPGEditorToolWindow.ShowTip("地形输出路径为空!", StatusBar.TipLevel.Warning);
				return;
			}

			bool enableLOD = m_terrainSetting.enableLOD;
			List<GameObject> terrainGOList = new List<GameObject>();
			foreach (var item in m_terrainGOList)
			{
				if (!item)
					continue;

				var TerrainGO = Utils.TerrainGenerator.CombineChunkMeshes(item, m_terrainSetting.material);
				Undo.RegisterCreatedObjectUndo(TerrainGO, "Create Prefab");

				terrainGOList.Add(TerrainGO);
			}
			if (terrainGOList.Count == 0)
				return;

			//保存Mesh
			SaveMesh(terrainGOList);

			//创建LODGroup
			GameObject terrainGO = enableLOD ? CreateLODGroup(terrainGOList) : terrainGOList[0];
			terrainGO.name = m_terrainSetting.terrainName;

			//保存prefab
			SavePrefab(terrainGO);

			//删除
			foreach (var terrain in terrainGOList)
			{
				if (terrain)
				{
					Undo.DestroyObjectImmediate(terrain);
				}
			}

			//删除预览
			foreach (var item in m_terrainGOList)
			{
				if (item)
				{
					Undo.DestroyObjectImmediate(item);
				}
			}
		}

		/// <summary>
		/// 保存mesh
		/// </summary>
		private void SaveMesh(List<GameObject> terrainGOList)
		{
			foreach (var terrainGO in terrainGOList)
			{
				var meshFilter = terrainGO.GetComponent<MeshFilter>();
				if (!meshFilter)
					continue;

				string meshName = $"{terrainGO.name}_Mesh";
				var mesh = UnityEngine.Object.Instantiate(meshFilter.sharedMesh);
				mesh.name = meshName;

				string meshPath = $"{m_view.TerrainSavePath}/{meshName}.asset";
				meshPath = AssetDatabase.GenerateUniqueAssetPath(meshPath);
				terrainGO.GetComponent<MeshFilter>().sharedMesh = mesh;
				AssetDatabase.CreateAsset(mesh, meshPath);

				RPGEditorToolWindow.ShowTip($"保存{terrainGO.name}Mesh成功!");
			}
		}

		/// <summary>
		/// 创建LODGroup
		/// </summary>
		private GameObject CreateLODGroup(List<GameObject> terrainGOList)
		{
			GameObject terrainGO = new GameObject();
			var lodPercent = GetLODPercents(m_terrainSetting.lodLevel);

			//添加LODGroup
			var lodGroup = terrainGO.AddComponent<LODGroup>();
			List<LOD> lods = new List<LOD>();
			for (int i = 0; i < terrainGOList.Count; i++)
			{
				var go = terrainGOList[i];
				go.transform.SetParent(terrainGO.transform);

				var renderer = go.GetComponent<MeshRenderer>();
				lods.Add(new LOD(lodPercent[i], new Renderer[] { renderer }));
			}
			lodGroup.SetLODs(lods.ToArray());
			lodGroup.RecalculateBounds();

			return terrainGO;
		}

		private void SavePrefab(GameObject terrainGO)
		{
			//保存prefab
			string prefabPath = $"{m_view.TerrainSavePath}/{terrainGO.name}.prefab";
			prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
			PrefabUtility.SaveAsPrefabAsset(terrainGO, prefabPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			RPGEditorToolWindow.ShowTip($"保存{terrainGO.name}成功!");

			Undo.DestroyObjectImmediate(terrainGO);

			//加载prefab
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
			PrefabUtility.InstantiatePrefab(prefab);
		}

		private void CreateSettingData(Model_PTGTool param)
		{
			m_terrainSetting.terrainName = param.terrainName;
			m_terrainSetting.material = param.terrainMaterial;

			//地形参数
			m_terrainSetting.terrainWidth = param.terrainWidth;
			m_terrainSetting.terrainLength = param.terrainLength;
			m_terrainSetting.chunkSize = param.chunkSize;
			m_terrainSetting.minHeight = param.minHeight;
			m_terrainSetting.maxHeight = param.maxHeight;
			m_terrainSetting.heightCurve = param.heightCurve;

			//噪声参数
			m_terrainSetting.noiseScale = param.noiseScale;
			m_terrainSetting.octaves = param.octaves;
			m_terrainSetting.persistence = param.persistence;
			m_terrainSetting.lacunarity = param.lacunarity;

			//LOD
			m_terrainSetting.enableLOD = param.enableLOD;
			m_terrainSetting.lodLevel = param.lodLevel + 1;
		}

		private void CleanInvalidPreview()
		{
			if (m_terrainGOList == null || m_terrainGOList.Count == 0)
				return;

			for (int i = m_terrainGOList.Count - 1; i >= 0; i--)
			{
				if (m_terrainGOList[i] == null)
				{
					m_terrainGOList.RemoveAt(i);
				}
			}
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
