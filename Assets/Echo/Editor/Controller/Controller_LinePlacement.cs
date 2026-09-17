using Echo.Component;
using Echo.Editor.Tool;
using Echo.Editor.UI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor
{
	public class Controller_LinePlacement : IDisposable
	{
		private View_LinePlacement m_view = null;
		private Model_LinePlacement m_viewModel = null;

		/// <summary>
		/// 资源销毁标识符
		/// </summary>
		private bool m_isDisposed = false;

		/// <summary>
		/// 已选曲线
		/// </summary>
		private List<GameObject> m_selectedCurves = null;
		/// <summary>
		/// 预览对象
		/// </summary>
		private Dictionary<GameObject, GameObject> m_previewGOMap = null;
		/// <summary>
		/// 模型放置位置
		/// </summary>
		private Dictionary<GameObject, List<Pose>> m_placePoseMap = null;
		private List<GameObject> m_placementGOs = null;

		/// <summary>
		/// 是否处于预览
		/// </summary>
		private bool m_isPreviewing = false;

		public Controller_LinePlacement(View_LinePlacement view)
		{
			m_view = view;
			//m_view.InitData(LinePlacementSettings.instance.viewData);

			m_selectedCurves = new List<GameObject>();
			m_previewGOMap = new Dictionary<GameObject, GameObject>();
			m_placePoseMap = new Dictionary<GameObject, List<Pose>>();
			m_placementGOs = new List<GameObject>();

			//订阅事件
			m_view.SelCurves += OnSelCurves;
			m_view.PreviewBtnClick += OnPreviewBtnClick;
			m_view.OkClick += OnOkClick;
			m_view.CancelClick += OnCancelClick;
			m_view.ValueChanged += OnValueChanged;
		}

		public void Dispose()
		{
			if (m_isDisposed)
				return;
			Debug.Log("LinePlacement Controller Dispose");

			//事件解绑
			m_view.SelCurves -= OnSelCurves;
			m_view.PreviewBtnClick -= OnPreviewBtnClick;
			m_view.OkClick -= OnOkClick;
			m_view.CancelClick -= OnCancelClick;
			m_view.ValueChanged	-= OnValueChanged;

			//删除预览
			ClearPreview();

			m_isDisposed = true;
		}

		#region Event Functions
		private void OnSelCurves()
		{
			//清空预览
			m_isPreviewing = false;
			ClearPreview();
			m_placePoseMap.Clear();

			EditorToolManager.SetTool(new SelectionTool("请选择曲线", true, result =>
			{
				m_selectedCurves.Clear();
				m_selectedCurves.AddRange(result);
				m_view.Label.text = $"已选曲线：{m_selectedCurves.Count}";
			}, typeof(PolyLine)));
		}

		private void OnPreviewBtnClick()
		{
			if (m_selectedCurves.Count == 0)
			{
				RPGEditorToolWindow.ShowTip("未选择布设曲线!", StatusBar.TipLevel.Warning);
				return;
			}

			m_viewModel = m_view.GetData();
			if (m_viewModel.placeModel == null)
			{
				RPGEditorToolWindow.ShowTip("未设置布设模型!", StatusBar.TipLevel.Warning);
				return;
			}

			//清空预览
			m_isPreviewing = true;
			ClearPreview();
			m_placePoseMap.Clear();
			GeneratePreview();
		}

		private void OnOkClick()
		{
			if (m_selectedCurves.Count == 0)
			{
				RPGEditorToolWindow.ShowTip("未选择布设曲线!", StatusBar.TipLevel.Warning);
				return;
			}

			m_viewModel = m_view.GetData();
			if (m_viewModel.placeModel == null)
			{
				RPGEditorToolWindow.ShowTip("未设置布设模型!", StatusBar.TipLevel.Warning);
				return;
			}

			//清空预览
			ClearPreview();
			//删除已创建的布设对象
			ClearPlacementObject();

			for (int i = 0; i < m_selectedCurves.Count; i++)
			{
				string name = (i == 0) ? m_viewModel.name : m_viewModel.name + $"_{i + 1}";
				var curveGO = m_selectedCurves[i];
				if (m_placePoseMap.TryGetValue(curveGO, out List<Pose> placePoses))
				{
					CreatePlacementGO(name, curveGO, placePoses);
				}
				else
				{
					CreatePlacementGO(name, curveGO);
				}
			}

			m_placePoseMap.Clear();
			m_isPreviewing = false;
		}

		private void OnCancelClick()
		{
			//清空预览
			ClearPreview();
			m_placePoseMap.Clear();
			m_isPreviewing = false;
		}

		private void OnValueChanged()
		{
			if (!m_isPreviewing)
				return;

			//更新预览
			RefreshPreview();
		}
		#endregion

		/// <summary>
		/// 生成预览
		/// </summary>
		private void GeneratePreview()
		{
			for (int i = 0; i < m_selectedCurves.Count; i++)
			{
				string name = (i == 0) ? m_viewModel.name : m_viewModel.name + $"_{i + 1}";
				var curveGO = m_selectedCurves[i];
				var placementGO = CreatePreviewGO(name, curveGO);
				if (placementGO == null)
				{
					continue;
				}
				m_previewGOMap[curveGO] = placementGO;
			}
		}

		/// <summary>
		/// 刷新预览
		/// </summary>
		private void RefreshPreview()
		{
			m_viewModel = m_view.GetData();
			if (m_viewModel.placeModel == null)
			{
				RPGEditorToolWindow.ShowTip("未设置布设模型!", StatusBar.TipLevel.Warning);
				return;
			}

			ClearPreview();
			m_placePoseMap.Clear();
			GeneratePreview();
		}

		/// <summary>
		/// 创建预览
		/// </summary>
		private GameObject CreatePreviewGO(string name, GameObject curveGO)
		{
			var curve = curveGO.GetComponent<Curve>();
			if (curve == null || curve.GetLength() == 0)
				return null;

			GameObject go = new GameObject(name);
			go.hideFlags = HideFlags.DontSaveInEditor;//预览对象不保存到场景中

			//添加沿线布设策略组件
			var lineStrategy = go.AddComponent<LinePlacementStrategy>();
			lineStrategy.Curve = curve;
			lineStrategy.Param = m_viewModel.param;
			lineStrategy.PlaceModel = m_viewModel.placeModel;

			//获取布设点
			List<Pose> placePoses= lineStrategy.Compute();
			if (placePoses == null || placePoses.Count == 0)
			{
				UnityEngine.Object.DestroyImmediate(go);
				return null;
			}

			//放置模型
			for (int i = 0; i < placePoses.Count; i++)
			{
				GameObject instance = PrefabUtility.InstantiatePrefab(m_viewModel.placeModel) as GameObject;
				if (instance == null)
					continue;

				instance.transform.SetParent(go.transform, true);
				instance.transform.SetPositionAndRotation(placePoses[i].position, placePoses[i].rotation);
				instance.hideFlags = HideFlags.DontSaveInEditor;
			}
			m_placePoseMap.Add(curveGO, placePoses);

			return go;
		}

		private void CreatePlacementGO(string name, GameObject curveGO)
		{
			var curve = curveGO.GetComponent<Curve>();
			if (curve == null || curve.GetLength() == 0)
				return;

			GameObject placementRootGO = new GameObject(name);

			//创建曲线子对象
			GameObject newCurveGO = CreateCurveGO(curveGO);
			if (newCurveGO == null)
			{
				UnityEngine.Object.DestroyImmediate(placementRootGO);
				return;
			}
			newCurveGO.transform.SetParent(placementRootGO.transform, true);

			//添加沿线布设策略组件
			var lineStrategy = placementRootGO.AddComponent<LinePlacementStrategy>();
			lineStrategy.Curve = newCurveGO.GetComponent<Curve>();
			lineStrategy.Param = m_viewModel.param;
			lineStrategy.PlaceModel = m_viewModel.placeModel;

			//获取布设点
			List<Pose> placePoses = lineStrategy.Compute();
			if (placePoses == null || placePoses.Count == 0)
			{
				UnityEngine.Object.DestroyImmediate(placementRootGO);
				return;
			}

			//放置模型
			GameObject placementGO = new GameObject("Placement Models");
			placementGO.transform.SetParent(placementRootGO.transform, true);
			for (int i = 0; i < placePoses.Count; i++)
			{
				GameObject instance = PrefabUtility.InstantiatePrefab(m_viewModel.placeModel) as GameObject;
				if (instance == null)
					continue;

				instance.transform.SetParent(placementGO.transform, true);
				instance.transform.SetPositionAndRotation(placePoses[i].position, placePoses[i].rotation);
			}
			m_placementGOs.Add(placementRootGO);
		}

		private void CreatePlacementGO(string name, GameObject curveGO, List<Pose> placePoses)
		{
			var curve = curveGO.GetComponent<Curve>();
			if (curve == null || curve.GetLength() == 0)
				return;

			GameObject placementRootGO = new GameObject(name);

			//创建曲线子对象
			GameObject newCurveGO = CreateCurveGO(curveGO);
			if (newCurveGO == null)
			{
				UnityEngine.Object.DestroyImmediate(placementRootGO);
				return;
			}
			newCurveGO.transform.SetParent(placementRootGO.transform, true);

			//添加沿线布设策略组件
			var lineStrategy = placementRootGO.AddComponent<LinePlacementStrategy>();
			lineStrategy.Curve = newCurveGO.GetComponent<Curve>();
			lineStrategy.Param = m_viewModel.param;
			lineStrategy.PlaceModel = m_viewModel.placeModel;

			//放置模型
			GameObject placementGO = new GameObject("Placement Models");
			placementGO.transform.SetParent(placementRootGO.transform, true);
			for (int i = 0; i < placePoses.Count; i++)
			{
				GameObject instance = PrefabUtility.InstantiatePrefab(m_viewModel.placeModel) as GameObject;
				if (instance == null)
					continue;

				instance.transform.SetParent(placementGO.transform, true);
				instance.transform.SetPositionAndRotation(placePoses[i].position, placePoses[i].rotation);
			}
			m_placementGOs.Add(placementRootGO);
		}

		/// <summary>
		/// 创建曲线对象
		/// </summary>
		private GameObject CreateCurveGO(GameObject targetGO)
		{
			if (targetGO == null)
				return null;

			var targetCurve = targetGO.GetComponent<Curve>();
			if (targetCurve == null)
				return null;

			//创建曲线对象
			GameObject curveGO = new GameObject(targetGO.name);
			curveGO.transform.SetPositionAndRotation(targetCurve.transform.position, targetCurve.transform.rotation);
			curveGO.transform.localScale = targetCurve.transform.lossyScale;

			//创建曲线组件
			Curve newCurve = curveGO.AddComponent(targetCurve.GetType()) as Curve;
			if (newCurve == null)
			{
				UnityEngine.Object.DestroyImmediate(curveGO);
				return null;
			}

			//复制组件
			EditorUtility.CopySerialized(targetCurve, newCurve);
			return curveGO;
		}

		/// <summary>
		/// 清理预览
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

		/// <summary>
		/// 清理创建的布设对象
		/// </summary>
		private void ClearPlacementObject()
		{
			for (int i = 0; i < m_placementGOs.Count; i++)
			{
				if (m_placementGOs[i] != null)
				{
					UnityEngine.Object.DestroyImmediate(m_placementGOs[i]);
				}
			}
			m_placementGOs.Clear();
		}
	}
}
