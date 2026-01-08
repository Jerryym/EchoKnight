using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 角色物理属性导入工具
/// </summary>
public class PlayerPhysicsConfigImportTool : EditorWindow
{
	private TextAsset m_csvFileName;
	private string m_outputPath = "Assets/GameData/Player/Configs";

	[MenuItem("Tools/Config Import/Player Physics Config")]
	static void Init()
	{
		PlayerPhysicsConfigImportTool window = (PlayerPhysicsConfigImportTool)GetWindow(typeof(PlayerPhysicsConfigImportTool), false, "角色物理属性导入工具");
		window.Show();
	}

	private void OnGUI()
	{
		EditorGUILayout.Space();
		GUILayout.Label("物理属性导入", EditorStyles.boldLabel);

		//选择文件
		TextAsset fileName = (TextAsset)EditorGUILayout.ObjectField("配置文件(.csv)", m_csvFileName, typeof(TextAsset), false);
		if (fileName != null)//验证扩展名
		{
			string path = AssetDatabase.GetAssetPath(fileName);
			if (!path.EndsWith(".csv", System.StringComparison.OrdinalIgnoreCase))
			{
				Debug.LogWarning($"所选文件 \"{fileName.name}\" 不是 .csv 文件，请重新选择。");
				fileName = null;
			}
		}
		m_csvFileName = fileName;

		m_outputPath = EditorGUILayout.TextField("保存路径", m_outputPath);
		EditorGUILayout.Space();

		using (new EditorGUI.DisabledScope(m_csvFileName == null))
		{
			if (GUILayout.Button("Import / Update Config"))
			{
				Import();
			}
		}
	}

	private void Import()
	{
		if (!Directory.Exists(m_outputPath))
		{
			Directory.CreateDirectory(m_outputPath);
		}

		string[] lines = m_csvFileName.text.Split('\n');
		for (int i = 0; i < lines.Length; i++)
		{
			string line = lines[i].Trim();
			if (i == 0 || string.IsNullOrWhiteSpace(line)) //表头或空行, 跳过
				continue;

			string[] tokens = lines[i].Trim().Split(',');
			if (tokens.Length < 6)
			{
				Debug.LogError($"配置行格式错误: {lines[i]}");
				continue;
			}

			//设置文件名
			string id = tokens[0];
			string assetPath = Path.Combine(m_outputPath, id + ".asset");

			//判断是否已存在同名SO资源
			PlayerPhysicsConfigSO config = AssetDatabase.LoadAssetAtPath<PlayerPhysicsConfigSO>(assetPath);
			if (config == null)
			{
				//创建对应SO文件
				config = ScriptableObject.CreateInstance<PlayerPhysicsConfigSO>();
				AssetDatabase.CreateAsset(config, assetPath);
			}

			if (!float.TryParse(tokens[1], out config.gravity))//重力
			{ 
				Debug.LogError($"gravity 解析失败, ID={id}"); 
				continue; 
			}
			if (!float.TryParse(tokens[2], out config.groundGravity))//地面重力
			{ 
				Debug.LogError($"groundGravity 解析失败, ID={id}"); 
				continue; 
			}
			if (!float.TryParse(tokens[3], out config.rotationFactorPerFrame))//每帧旋转速度
			{ 
				Debug.LogError($"rotationFactorPerFrame 解析失败, ID={id}"); 
				continue; 
			}
			if (!float.TryParse(tokens[4], out config.maxJumpHeight))//最大跳跃高度
			{ 
				Debug.LogError($"maxJumpHeight 解析失败, ID={id}"); 
				continue; 
			}
			if (!float.TryParse(tokens[5], out config.maxJumpTime))//最大跳跃时间
			{ 
				Debug.LogError($"maxJumpTime 解析失败, ID={id}"); 
				continue; 
			}
		}
		//本地化资源
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		Debug.Log("导入成功.");
	}
}
