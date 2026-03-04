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
				var terrainGO = TerrainGenerator.GenerateTerrain(param.terrainName, m_terrainSetting);
				terrainGO.hideFlags = HideFlags.DontSave;
				m_terrainGOList.Add(terrainGO);
			}
			else
			{
				m_terrainGOList = TerrainGenerator.GenerateTerrainLODs(param.terrainName, m_terrainSetting);
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

			foreach (var item in m_terrainGOList)
			{
				var TerrainGO = TerrainGenerator.CombineChunkMeshes(item, m_terrainSetting.material);
				Undo.RegisterCreatedObjectUndo(TerrainGO, "Create Prefab");

				//保存mesh
				string meshName = $"{TerrainGO.name}_Mesh";
				var mesh = UnityEngine.Object.Instantiate(TerrainGO.GetComponent<MeshFilter>().sharedMesh);
				mesh.name = meshName;

				string meshPath = $"{m_view.TerrainSavePath}/{meshName}.asset";
				meshPath = AssetDatabase.GenerateUniqueAssetPath(meshPath);
				TerrainGO.GetComponent<MeshFilter>().sharedMesh = mesh;
				AssetDatabase.CreateAsset(mesh, meshPath);

				//保存prefab
				string prefabPath = $"{m_view.TerrainSavePath}/{TerrainGO.name}.prefab";
				prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
				PrefabUtility.SaveAsPrefabAsset(TerrainGO, prefabPath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				RPGEditorToolWindow.ShowTip($"保存{TerrainGO.name}成功!");

				Undo.DestroyObjectImmediate(TerrainGO);
				
				//加载prefab
				GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
				PrefabUtility.InstantiatePrefab(prefab);
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

		private void CreateSettingData(Model_PTGTool param)
		{
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
	}
}
