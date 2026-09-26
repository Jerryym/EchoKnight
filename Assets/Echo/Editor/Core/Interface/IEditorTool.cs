using UnityEditor;

namespace Echo.Editor
{
	/// <summary>
	/// 编辑器工具接口
	/// </summary>
	public interface IEditorTool
	{
		/// <summary>
		/// 激活: 用于初始化资源、注册事件或设置初始状态
		/// </summary>
		internal void Activate();
		/// <summary>
		/// 停用: 用于释放资源、事件解绑
		/// </summary>
		internal void DeActivate();
		/// <summary>
		/// 在场景视图刷新时调用
		/// </summary>
		/// <param name="sceneView"></param>
		internal void OnSceneGUI(SceneView sceneView);
	}
}
