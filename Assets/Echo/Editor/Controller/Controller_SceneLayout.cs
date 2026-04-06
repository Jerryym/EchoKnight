using System;
using UnityEngine;

namespace Echo.Editor
{
	public class Controller_SceneLayout : IDisposable
	{
		private View_SceneLayout m_view = null;

		/// <summary>
		/// 资源销毁标识符
		/// </summary>
		private bool m_isDisposed = false;

		public Controller_SceneLayout(View_SceneLayout view)
		{
			m_view = view;

			//订阅事件
			m_view.OnToolButtonClicked += OnToolButtonClicked;
		}

		public void Dispose()
		{
			if (m_isDisposed)
				return;
			Debug.Log("Scene Layout Controller Dispose");

			//事件解绑
			m_view.OnToolButtonClicked -= OnToolButtonClicked;

			m_isDisposed = true;
		}

		private void OnToolButtonClicked()
		{
			PlacementMode mode = m_view.CurrentMode;
			switch (mode)
			{
				case PlacementMode.Line://沿线布设
					CreateLinePlacementView();
					break;
				case PlacementMode.Area://区域布设
					CreateAreaPlacementView();
					break;
				case PlacementMode.Shader://Shader布设
					CreateShaderPlacementView();
					break;
				default:
					break;
			}
		}

		private void CreateLinePlacementView()
		{
			Debug.Log("沿线布设");
		}

		private void CreateAreaPlacementView()
		{
			Debug.Log("区域布设");
		}

		private void CreateShaderPlacementView()
		{
			Debug.Log("Shader布设");
		}
	}
}
