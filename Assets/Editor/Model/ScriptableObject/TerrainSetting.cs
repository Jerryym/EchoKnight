using UnityEngine;

namespace Echo.Editor
{
	[CreateAssetMenu(menuName = "PTG/Terrain Setting")]
	public class TerrainSetting : ScriptableObject
	{
		public Material material;

		#region 地形尺寸
		/// <summary>
		/// 地形横向宽度(X)
		/// </summary>
		[Min(1)] public int terrainWidth = 256;
		/// <summary>
		/// 地形纵向宽度(Z)
		/// </summary>
		[Min(1)] public int terrainLength = 256;
		/// <summary>
		/// 分块大小
		/// </summary>
		[Min(1)] public int chunkSize = 32;
		#endregion

		#region 高度设置
		/// <summary>
		/// 地形最小高度
		/// </summary>
		[Range(1f, 200f)] public float minHeight = 0.0f;
		/// <summary>
		/// 地形最大高度
		/// </summary>
		[Range(1f, 200f)] public float maxHeight = 20f;
		/// <summary>
		/// 高度曲线
		/// </summary>
		public AnimationCurve heightCurve = new AnimationCurve(
			new Keyframe(0, 0),
			new Keyframe(0.3f, 0.1f),
			new Keyframe(0.6f, 0.8f),
			new Keyframe(1f, 1f)
		);
		#endregion

		#region 噪声参数
		/// <summary>
		/// 噪声缩放
		/// </summary>
		[Range(1f, 500f)] public float noiseScale = 50.0f;
		/// <summary>
		/// 噪声层数
		/// </summary>
		[Range(1, 8)] public int octaves = 4;
		/// <summary>
		/// 持久度
		/// </summary>
		[Range(0f, 1f)] public float persistence = 0.5f;
		/// <summary>
		/// 空隙度
		/// </summary>
		[Range(1f, 4f)] public float lacunarity = 2.0f;
		#endregion

		#region LOD
		/// <summary>
		/// 设置LOD
		/// </summary>
		public bool enableLOD = true;
		/// <summary>
		/// LOD等级
		/// </summary>
		public int lodLevel = 1;
		#endregion
	}
}
