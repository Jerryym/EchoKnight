using Echo.Component;
using Echo.Editor.Tool;
using System;
using System.Collections.Generic;
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

		public Controller_LinePlacement(View_LinePlacement view)
		{
			m_view = view;
			m_selectedCurves = new List<GameObject>();

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
			if (m_selectedCurves.Count == 0)
				return;
		}

		private void OnOkClick()
		{
			throw new NotImplementedException();
		}

		private void OnCancelClick()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
