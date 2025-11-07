using UnityEngine;

/// <summary>
/// 地形生成器
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
	[Header("地形属性")]
	/// <summary>
	/// 地形在X轴上的大小(宽度)
	/// </summary>
	[Tooltip("地形在X轴上的大小(宽度)")]
	public int xSize;
	/// <summary>
	/// 地形在Z轴上的大小(长度)
	/// </summary>
	[Tooltip("地形在Z轴上的大小(长度)")]
	public int zSize;
	/// <summary>
	/// 最小高度
	/// </summary>
	public float minHeight;
	/// <summary>
    /// 最大高度
    /// </summary>
	public float maxHeight;

	[Header("地形分块属性")]
	/// <summary>
    /// 分块大小
    /// </summary>
	public int chunckSize = 32;
	/// <summary>
	/// 分块数量(X方向)
	/// </summary>
	public int chunkCountX;
	/// <summary>
    /// 分块数量(Z方向)
    /// </summary>
	public int chunkCountZ;

	/// <summary>
    /// 地形网格
    /// </summary>
	private Mesh m_terrainMesh = null;
	/// <summary>
	/// 顶点数组
	/// </summary>
	private Vector3[] m_vertices;
	/// <summary>
	/// 索引数组
	/// </summary>
	private int[] m_triangles;
	
	private void OnDrawGizmos()
	{
		if (m_vertices == null)
			return;

		//绘制顶点
		for (int i = 0; i < m_vertices.Length; i++)
        {
			Gizmos.DrawSphere(m_vertices[i], 0.1f);
        }
    }

	/// <summary>
	/// 生成地形
	/// </summary>
	public void GenerateTerrain()
	{
		m_terrainMesh = new Mesh();
		m_terrainMesh.name = "TerrainMesh";
		GetComponent<MeshFilter>().mesh = m_terrainMesh;

		CreateShape();
		UpdateMesh();
	}

	private void CreateShape()
	{
		//创建顶点数组
		m_vertices = new Vector3[(xSize + 1) * (zSize + 1)];
		int index = 0;
		for (int z = 0; z <= zSize; z++)
		{
			for (int x = 0; x <= xSize; x++)
			{
				float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2f;
				m_vertices[index] = new Vector3(x, y, z);
				index++;
			}
		}
		Debug.Log("顶点数量：" + m_vertices.Length);

		//创建索引数组
		m_triangles = new int[xSize * zSize * 6];
		int triIndex = 0, verIndex = 0;
		for (int z = 0; z < zSize; z++)
        {
			for (int x = 0; x < xSize; x++)
			{
				//第一个三角形
				m_triangles[triIndex + 0] = verIndex + 0;
				m_triangles[triIndex + 1] = verIndex + xSize + 1;
				m_triangles[triIndex + 2] = verIndex + 1;

				//第二个三角形
				m_triangles[triIndex + 3] = verIndex + 1;
				m_triangles[triIndex + 4] = verIndex + xSize + 1;
				m_triangles[triIndex + 5] = verIndex + xSize + 2;

				verIndex++;
				triIndex += 6;
			}
			verIndex++;
        }
		Debug.Log("索引数量：" + m_triangles.Length);
	}
	
	private void UpdateMesh()
    {
		m_terrainMesh.Clear();
		m_terrainMesh.vertices = m_vertices;
		m_terrainMesh.triangles = m_triangles;
		m_terrainMesh.RecalculateNormals();
    }
}
