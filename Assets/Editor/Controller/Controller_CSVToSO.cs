using Echo.Editor.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Echo.Editor
{
	public class Controller_CSVToSO : IDisposable
	{
		private readonly RPGEditorToolWindow activeWindow = RPGEditorToolWindow.ActiveWindow;
		private View_CSVToSO m_view;

		/// <summary>
		/// 资源销毁标识符
		/// </summary>
		private bool m_isDisposed = false;

		private Type m_currentConfigType;
		private List<CharacterPhysicsConfigData> m_characterPhysicsConfigs = null;

		public Controller_CSVToSO(View_CSVToSO view)
		{ 
			m_view = view;
			m_view.InitData(CSVToSOSettings.instance.viewData);//初始化数据

			//订阅事件
			m_view.OnCSVFileChanged += ReadCSVFile;
			m_view.OnConfigChanged += InitTableTitle;
			m_view.OnGenerateClicked += GenerateSO;
			m_view.OnUpdateClicked += UpdateConfig;

			UpdateTable();
		}

		public void Dispose()
		{
			if (m_isDisposed)
				return;
			Debug.Log("Controller_CSVToSO Dispose");

			//事件解绑
			m_view.OnCSVFileChanged -= ReadCSVFile;
			m_view.OnConfigChanged -= InitTableTitle;
			m_view.OnGenerateClicked -= GenerateSO;
			m_view.OnUpdateClicked -= UpdateConfig;
			
			m_isDisposed = true;
		}

		private void UpdateTable()
		{
			int index = CSVToSOSettings.instance.viewData.configIndex;
			InitTableTitle(m_view.SOCofigTypes[index]);
		}

		private void ReadCSVFile(TextAsset textAsset)
		{
			if (m_currentConfigType == typeof(CharacterPhysicsConfigSO))//角色物理属性
			{
				if (m_characterPhysicsConfigs == null)
					m_characterPhysicsConfigs = new List<CharacterPhysicsConfigData>();

				m_characterPhysicsConfigs.Clear();
				ReadCharacterPhysicsConfig(textAsset);
			}
		}

		private void ReadCharacterPhysicsConfig(TextAsset textAsset)
		{
			string[] configs = textAsset.text.Split('\n');
			for (int i = 0; i < configs.Length; i++)
			{
				string data = configs[i].Trim();
				if (i == 0 || string.IsNullOrWhiteSpace(data))
					continue;

				string[] configDatas = data.Split(",");
				if (configDatas.Length < 6)
				{
					ShowTip($"配置文件第{i}行格式错误: {data}", StatusBar.TipLevel.Error);
					continue;
				}

				try
				{
					CharacterPhysicsConfigData physicsConfigData = new CharacterPhysicsConfigData();
					physicsConfigData.characterType = configDatas[0];
					physicsConfigData.gravity = ParseFloat(configDatas[1], i, "gravity");
					physicsConfigData.groundGravity = ParseFloat(configDatas[2], i, "groundGravity");
					physicsConfigData.rotationFactorPerFrame = ParseFloat(configDatas[3], i, "rotationFactorPerFrame");
					physicsConfigData.maxJumpHeight = ParseFloat(configDatas[4], i, "maxJumpHeight");
					physicsConfigData.maxJumpTime = ParseFloat(configDatas[5], i, "maxJumpTime");

					m_characterPhysicsConfigs.Add(physicsConfigData);
				}
				catch (Exception e)
				{
					ShowTip($"第{i + 1}行解析失败: {e.Message}", StatusBar.TipLevel.Error);
				}
			}
			ShowTip($"CSV解析完成,共读取{m_characterPhysicsConfigs.Count}条数据");
		}

		private void InitTableTitle(Type configType)
		{
			var fields = configType.GetFields(BindingFlags.Public | BindingFlags.Instance);
			if (fields.Length == 0)
				return;

			m_currentConfigType = configType;
			List<string> headers = new List<string>();
			foreach (var field in fields)
			{
				var attr = field.GetCustomAttribute<ConfigFieldAttribute>();
				string displayName = (attr != null) ? attr.FieldName : field.Name;
				headers.Add(displayName);
			}

			if (headers.Count == 0)
				return;

			m_view.UpdateTable(headers);
		}

		private void GenerateSO()
		{
		}

		private void UpdateConfig()
		{
		}

		private CharacterPhysicsConfigSO ConvertDataToSO(CharacterPhysicsConfigData data)
		{
			var soConfig = ScriptableObject.CreateInstance<CharacterPhysicsConfigSO>();
			soConfig.characterType = data.characterType;
			soConfig.gravity = data.gravity;
			soConfig.groundGravity = data.groundGravity;
			soConfig.rotationFactorPerFrame = data.rotationFactorPerFrame;
			soConfig.maxJumpHeight = data.maxJumpHeight;
			soConfig.maxJumpTime = data.maxJumpTime;
			return soConfig;
		}

		/// <summary>
		/// 显示提示
		/// </summary>
		private void ShowTip(string msg, StatusBar.TipLevel level = StatusBar.TipLevel.Info)
		{
			activeWindow.SetStatusBarText(msg, level);
		}

		private float ParseFloat(string value, int rowIndex, string fieldName)
		{
			if (!float.TryParse(value, out float result))
				throw new Exception($"第{rowIndex + 1}行字段 {fieldName} 不是合法float: {value}");

			return result;
		}
	}
}
