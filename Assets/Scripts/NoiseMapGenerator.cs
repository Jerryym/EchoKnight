using UnityEngine;

/// <summary>
/// 噪声图生成器
/// </summary>
public class NoiseMapGenerator : MonoBehaviour
{
	public Renderer textureRenderer;

	public int width;
	public int height;
	public float noiseScale;

	public void GenerateNoiseMap()
	{
		var noiseMap = Noise.GenerateNoiseMap(width, height, noiseScale);
		DrawNoiseMap(noiseMap);
	}

	private void DrawNoiseMap(float[,] noiseMap)
	{
		int mapWidth = noiseMap.GetLength(0);
		int mapHeight = noiseMap.GetLength(1);
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
