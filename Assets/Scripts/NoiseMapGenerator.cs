using UnityEngine;

/// <summary>
/// 噪声图生成器
/// </summary>
public class NoiseMapGenerator : MonoBehaviour
{
	public Renderer textureRenderer;

	/// <summary>
	/// 地图宽度
	/// </summary>
	public int width;
	/// <summary>
	/// 地图高度
	/// </summary>
	public int height;
	/// <summary>
	/// 噪声缩放因子
	/// </summary>
	[Range(0f, 10f)]
	public float noiseScale;

	public void GenerateNoiseMap()
	{
		float offsetX = Random.Range(0f, 10000f);
		float offsetY = Random.Range(0f, 10000f);
		var noiseMap = Noise.PerlinNoiseMap(width, height, noiseScale, offsetX, offsetY, -1, 1);
		DrawNoiseMap(noiseMap);
	}

	private void DrawNoiseMap(float[,] noiseMap)
	{
		int mapWidth = noiseMap.GetLength(0);
		int mapHeight = noiseMap.GetLength(1);
		Debug.Log("width = " + mapWidth + " height = " + mapHeight);
		
		//创建纹理
		Texture2D texture = new Texture2D(mapWidth, mapWidth);
		Color[] colorMap = new Color[mapWidth * mapWidth];
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				colorMap[y * mapWidth + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
			}
		}
		texture.SetPixels(colorMap);
		texture.Apply();

		textureRenderer.sharedMaterial.mainTexture = texture;
		textureRenderer.transform.localScale = new Vector3(mapWidth, 1, mapWidth);
	}
}
