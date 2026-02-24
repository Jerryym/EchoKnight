using System.Collections.Generic;
using System.Xml;

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
	}
}
