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

		/// <summary>
		/// 资源销毁标识符
		/// </summary>
		private bool m_isDisposed = false;

		/// <summary>
		/// 已选曲线
		/// </summary>
		private List<GameObject> m_selectedCurves = null;

		private List<GameObject> m_previewGOs = null;

		public Controller_LinePlacement(View_LinePlacement view)
		{
			m_view = view;
			//m_view.InitData(LinePlacementSettings.instance.viewData);

			m_selectedCurves = new List<GameObject>();
			m_previewGOs = new List<GameObject>();

			//订阅事件
			m_view.SelCurves += OnSelCurves;
			m_view.DrawAndPlaceBtnClick += OnDrawAndPlaceBtnClick;
			m_view.PreviewBtnClick += OnPreviewBtnClick;
			m_view.OkClick += OnOkClick;
			m_view.CancelClick += OnCancelClick;
		}

		public void Dispose()
		{
			if (m_isDisposed)
				return;
			Debug.Log("LinePlacement Controller Dispose");

			//事件解绑
			m_view.SelCurves -= OnSelCurves;
			m_view.DrawAndPlaceBtnClick -= OnDrawAndPlaceBtnClick;
			m_view.PreviewBtnClick -= OnPreviewBtnClick;
			m_view.OkClick -= OnOkClick;
			m_view.CancelClick -= OnCancelClick;

			m_isDisposed = true;
		}

		#region Event Functions
		private void OnSelCurves()
		{
			EditorToolManager.SetTool(new SelectionTool("请选择曲线", true, result =>
			{
				m_selectedCurves.Clear();
				m_selectedCurves.AddRange(result);
				m_view.Label.text = $"已选曲线：{m_selectedCurves.Count}";
			}, typeof(PolyLine)));
		}

		private void OnDrawAndPlaceBtnClick()
		{
			throw new NotImplementedException();
		}

		private void OnPreviewBtnClick()
		{
			//清空预览
			ClearPreview();

			var viewModel = m_view.GetData();
			if (viewModel.placeModel == null)
			{
				RPGEditorToolWindow.ShowTip("未设置布设模型!", StatusBar.TipLevel.Warning);
				return;
			}

			if (m_selectedCurves.Count == 0)
			{
				RPGEditorToolWindow.ShowTip("未选择布设曲线!", StatusBar.TipLevel.Warning);
				return;
			}

			Generate(viewModel);
		}

		private void OnOkClick()
		{
			throw new NotImplementedException();
		}

		private void OnCancelClick()
		{
			//清空预览
			ClearPreview();
		}
		#endregion

		private void Generate(Model_LinePlacement viewModel)
		{
			int index = 0;
			for (int i = 0; i < m_selectedCurves.Count; i++)
			{
				string name = viewModel.name;
				if (i != 0)
					name += $"_{index}";
				
				var curveGO = m_selectedCurves[i];
				var placementGO = CreatePreviewGO(name, curveGO, viewModel.param, viewModel.placeModel);
				if (placementGO == null)
				{
					continue;
				}
				m_previewGOs.Add(placementGO);
				index++;
			}
		}

		/// <summary>
		/// 创建预览
		/// </summary>
		/// <param name="name"></param>
		/// <param name="curveGO"></param>
		/// <returns></returns>
		private GameObject CreatePreviewGO(string name, GameObject curveGO, LinePlacementParam param, GameObject placeModel)
		{
			var curve = curveGO.GetComponent<Curve>();
			if (curve == null || curve.GetLength() == 0)
				return null;

			GameObject go = new GameObject(name);
			go.hideFlags = HideFlags.DontSaveInEditor;//预览对象不保存到场景中

			//添加沿线布设策略组件
			var lineStrategy = go.AddComponent<LinePlacementStrategy>();
			lineStrategy.Curve = curve;
			lineStrategy.Param = param;
			lineStrategy.PlaceModel = placeModel;

			//获取布设点
			IReadOnlyList<Vector3> placePts = lineStrategy.Compute();
			if (placePts == null || placePts.Count == 0)
			{
				UnityEngine.Object.DestroyImmediate(go);
				return null;
			}

			//放置模型
			for (int i = 0; i < placePts.Count; i++)
			{
				GameObject instance = PrefabUtility.InstantiatePrefab(placeModel) as GameObject;
				if (instance == null)
					continue;

				instance.transform.SetParent(go.transform, true);
				instance.transform.position = placePts[i];
				instance.hideFlags = HideFlags.DontSaveInEditor;
			}

			return go;
		}

		/// <summary>
		/// 清理预览
		/// </summary>
		private void ClearPreview()
		{
			for (int i = 0; i < m_previewGOs.Count; i++)
			{
				if (m_previewGOs[i] != null)
				{
					UnityEngine.Object.DestroyImmediate(m_previewGOs[i]);
				}
			}
			m_previewGOs.Clear();
		}
	}
}
