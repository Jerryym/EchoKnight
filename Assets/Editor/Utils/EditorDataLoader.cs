using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Echo.Editor.Utils
{
	public static class EditorDataLoader
	{
		/// <summary>
		/// 加载命令XML文件
		/// </summary>
		/// <param name="xmlFilePath"></param>
		/// <returns></returns>
		public static List<CommandInfo> LoadCommandXML(string xmlFilePath)
		{
			List<CommandInfo> commandInfos = new List<CommandInfo>();
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(xmlFilePath);

			foreach (XmlNode groupNode in xmlDoc.SelectNodes("/ToolBox/Group"))
			{
				string groupName = groupNode.Attributes["name"].Value;
				foreach (XmlNode cmdNode in groupNode.SelectNodes("Command"))
				{
					commandInfos.Add(new CommandInfo { group = groupName, id = cmdNode.Attributes["id"].Value, name = cmdNode.Attributes["name"].Value });
				}
			}

			return commandInfos;
		}

		/// <summary>
		/// 保存CSV
		/// </summary>
		/// <param name="csvPath"></param>
		/// <param name="tableModel"></param>
		/// <returns></returns>
		public static bool SaveCSV(string csvPath, TableModel tableModel)
		{
			if (string.IsNullOrEmpty(csvPath) || tableModel == null)
				return false;

			try
			{
				List<string> lines = new List<string>();

				//表头
				List<string> headers = new List<string>(tableModel.GetHeaders());
				lines.Add(string.Join(",", headers));

				//数据
				for (int r = 0; r < tableModel.RowCount; r++)
				{
					List<string> row = new List<string>();
					for (int c = 0; c < tableModel.ColumnCount; c++)
					{
						string value = tableModel.GetValue(r, c)?.ToString() ?? "";
						if (value.Contains(",") || value.Contains("\""))
						{
							value = $"\"{value.Replace("\"", "\"\"")}\"";
						}
						row.Add(value);
					}
					lines.Add(string.Join(",", row));
				}

				//写入文件
				File.WriteAllLines(csvPath, lines, System.Text.Encoding.UTF8);
				AssetDatabase.Refresh();
				return true;
			}
			catch (Exception e)
			{
				Debug.LogError($"CSV写入失败: {e}");
				return false;
			}
		}
	}
}
