using System;
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
			m_view.onPreviewBtnClick += OnPreviewBtnClick;
			m_view.onOkClick += OnOkClick;
			m_view.onCancelClick += OnCancelClick;
		}

		public void Dispose()
		{
			if (m_isDisposed)
				return;
			Debug.Log("LinePlacement Controller Dispose");

			//事件解绑
			m_view.onPreviewBtnClick -= OnPreviewBtnClick;
			m_view.onOkClick -= OnOkClick;
			m_view.onCancelClick -= OnCancelClick;

			m_isDisposed = true;
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
