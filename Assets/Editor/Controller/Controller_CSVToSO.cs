using Echo.Editor.UI;
using Echo.Editor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor
{
	public class Controller_CSVToSO : IDisposable
	{
		private View_CSVToSO m_view;

		/// <summary>
		/// 资源销毁标识符
		/// </summary>
		private bool m_isDisposed = false;

		private Type m_currentConfigType;
		private TableModel m_tableModel = null;
		private TextAsset m_csvAsset = null;

		public Controller_CSVToSO(View_CSVToSO view)
		{ 
			m_view = view;
			m_view.InitData(CSVToSOSettings.instance.viewData);//初始化数据

			m_tableModel = new TableModel();

			//订阅事件
			m_view.OnConfigChanged += InitTable;
			m_view.OnCSVFileChanged += ReadCSVFile;
			m_view.OnGenerateClicked += GenerateSO;
			m_view.OnUpdateClicked += UpdateCSVFile;

			UpdateTable();
		}

		public void Dispose()
		{
			if (m_isDisposed)
				return;
			Debug.Log("Controller_CSVToSO Dispose");

			//事件解绑
			m_view.OnConfigChanged -= InitTable;
			m_view.OnCSVFileChanged -= ReadCSVFile;
			m_view.OnGenerateClicked -= GenerateSO;
			m_view.OnUpdateClicked -= UpdateCSVFile;
			
			m_isDisposed = true;
		}

		private void UpdateTable()
		{
			int index = CSVToSOSettings.instance.viewData.configIndex;
			InitTable(m_view.SOCofigTypes[index]);
		}

		private void InitTable(Type configType)
		{
			//当前配置类型
			m_currentConfigType = configType;
			m_view.ClearTable();
		}

		private void ReadCSVFile(TextAsset textAsset)
		{
			List<TableWidget.ColumnItem> columnItems = new List<TableWidget.ColumnItem>();
			if (m_currentConfigType == typeof(CharacterPhysicsConfigSO))//角色物理属性
			{
				//解析csv
				LoadCharacterPhysicsConfig(textAsset, ref columnItems);
			}
			m_csvAsset = textAsset;

			//更新表格
			if (columnItems.Count == 0)
				return;
			m_view.UpdateTable(columnItems, m_tableModel);
		}

		private void GenerateSO()
		{
			m_view.GetTableData(out m_tableModel);
			if (m_tableModel.RowCount == 0)
			{
				RPGEditorToolWindow.ShowTip($"当前表格中无数据, 无法生成对应SO资源");
				return;
			}

			if (m_currentConfigType == typeof(CharacterPhysicsConfigSO))//角色物理属性
			{
				GenerateCharacterPhysicsConfigSO();
			}
		}

		private void GenerateCharacterPhysicsConfigSO()
		{
			for (int i = 0; i < m_tableModel.RowCount; i++)
			{
				string characterName = m_tableModel.GetValue(i, 0).ToString();
				if (string.IsNullOrEmpty(characterName))
				{
					RPGEditorToolWindow.ShowTip($"第{i + 1}行 角色类型 为空!", StatusBar.TipLevel.Warning);
					continue;
				}

				string assetPath = Path.Combine(m_view.ConfigSavePath, characterName + ".asset");
				var configSO = AssetDatabase.LoadAssetAtPath<CharacterPhysicsConfigSO>(assetPath);
				if (configSO == null)
				{
					configSO = ScriptableObject.CreateInstance<CharacterPhysicsConfigSO>();
					AssetDatabase.CreateAsset(configSO, assetPath);
				}

				//赋值
				SetCharacterPhysicsConfig(m_tableModel.Rows[i], configSO);
			}

			//保存
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			RPGEditorToolWindow.ShowTip($"生成SO资源成功，共生成 {m_tableModel.RowCount} 条");

			//保存界面数据
			CSVToSOSettings.instance.viewData = m_view.GetData();
			CSVToSOSettings.instance.Save();
		}

		private void UpdateCSVFile()
		{
			m_view.GetTableData(out m_tableModel);
			if (m_tableModel.RowCount == 0)
			{
				RPGEditorToolWindow.ShowTip("当前表格中无数据，无法更新CSV文件");
				return;
			}

			string csvPath = AssetDatabase.GetAssetPath(m_csvAsset);
			if (string.IsNullOrEmpty(csvPath))
			{
				RPGEditorToolWindow.ShowTip("CSV文件路径无效");
				return;
			}

			//保存
			bool success = EditorDataLoader.SaveCSV(csvPath, m_tableModel);
			if (success)
				RPGEditorToolWindow.ShowTip($"CSV文件更新成功 ({m_tableModel.RowCount} 条数据)");
			else
				RPGEditorToolWindow.ShowTip("更新CSV文件失败，请查看控制台");

			//保存界面数据
			CSVToSOSettings.instance.viewData = m_view.GetData();
			CSVToSOSettings.instance.Save();
		}

		private void LoadCharacterPhysicsConfig(TextAsset textAsset, ref List<TableWidget.ColumnItem> columnItems)
		{
			//初始化表头
			if (!InitTableTitle())
				return;

			//设置列项属性
			for (int i = 0; i < m_tableModel.ColumnCount; i++)
			{
				TableWidget.ColumnItem item = new TableWidget.ColumnItem { Type = TableWidget.ColumnType.Edit };
				columnItems.Add(item);
			}

			string[] configs = textAsset.text.Split('\n');
			for (int i = 0; i < configs.Length; i++)
			{
				string data = configs[i].Trim();
				if (i == 0 || string.IsNullOrWhiteSpace(data))
					continue;

				string[] configDatas = data.Split(",");
				if (configDatas.Length < 6)
				{
					RPGEditorToolWindow.ShowTip($"配置文件第{i}行格式错误: {data}", StatusBar.TipLevel.Error);
					continue;
				}

				m_tableModel.AddRow(configDatas);
			}
			RPGEditorToolWindow.ShowTip($"CSV解析完成, 共读取{m_tableModel.RowCount}条数据");
		}

		private bool InitTableTitle()
		{
			var fields = m_currentConfigType.GetFields(BindingFlags.Public | BindingFlags.Instance);
			if (fields.Length == 0)
				return false;

			m_tableModel.Clear();
			foreach (var field in fields)
			{
				var attr = field.GetCustomAttribute<ConfigFieldAttribute>();
				string title = (attr != null) ? attr.FieldName : field.Name;
				m_tableModel.AddColumn(title, typeof(string));
			}

			if (m_tableModel.ColumnCount == 0)
				return false;

			return true;
		}

		private void SetCharacterPhysicsConfig(TableModel.DataRow row, CharacterPhysicsConfigSO so)
		{
			so.characterType = row[0].ToString();
			so.gravity = ParseFloat(row[1], 0f);
			so.groundGravity = ParseFloat(row[2], 0f);
			so.rotationFactorPerFrame = ParseFloat(row[3], 0f);
			so.maxJumpHeight = ParseFloat(row[4], 0f);
			so.maxJumpTime = ParseFloat(row[5], 0f);
		}

		private float ParseFloat(object obj, float defaultValue)
		{
			if (obj == null)
				return defaultValue;
			if (float.TryParse(obj.ToString(), out float result)) 
				return result;

			return defaultValue;
		}
	}
}
