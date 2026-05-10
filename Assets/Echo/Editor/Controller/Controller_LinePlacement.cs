using System;
using Echo.Component;
using Echo.Editor.Tool;
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

		public Controller_LinePlacement(View_LinePlacement view)
		{
			m_view = view;

			//订阅事件
			m_view.SelCurves += OnSelCurves;
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
			m_view.PreviewBtnClick -= OnPreviewBtnClick;
			m_view.OkClick -= OnOkClick;
			m_view.CancelClick -= OnCancelClick;

			m_isDisposed = true;
		}

		private void OnSelCurves()
		{
			Debug.Log("选择曲线");

			EditorToolManager.SetTool(new SelectionTool("请选择曲线", true, result =>
			{
				Debug.Log($"已选中曲线数：{result.Count}");
			}, typeof(PolyLine)));
		}

		private void OnPreviewBtnClick()
		{
			throw new NotImplementedException();
		}

		private void OnOkClick()
		{
			throw new NotImplementedException();
		}

		private void OnCancelClick()
		{
			throw new NotImplementedException();
		}
	}
}
