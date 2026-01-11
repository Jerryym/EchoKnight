using UnityEngine;

public static class Noise
{
	/// <summary>
	/// Perlin噪声高度图
	/// </summary>
	/// <param name="mapWidth">地图宽度</param>
	/// <param name="mapHeight">地图高度</param>
	/// <param name="scale">噪声缩放因子</param>
	/// <param name="offsetX">噪声偏移X</param>
	/// <param name="offsetY">噪声偏移Y</param>
	/// <param name="minHeight">最小高度（世界坐标）</param>
	/// <param name="maxHeight">最大高度（世界坐标）</param>
	/// <returns></returns>
	public static float[,] PerlinNoiseMap(int mapWidth, int mapHeight, float scale, float offsetX, float offsetY, float minHeight, float maxHeight)
	{
		if (scale <= 0)
			scale = 0.001f;

		float[,] noiseMap = new float[mapWidth, mapHeight];
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				float xCoord = (x + offsetX) / scale;
				float yCoord = (y + offsetY) / scale;
				// 得到噪声值[0, 1]
				float noiseValue = Mathf.PerlinNoise(xCoord, yCoord);
				// 映射到[minHeight, maxHeight]
				float heightValue = Mathf.Lerp(minHeight, maxHeight, noiseValue);
				noiseMap[x, y] = heightValue;
			}
		}
		return noiseMap;
	}

	/// <summary>
	/// 多层（Octave）Perlin噪声高度图
	/// </summary>
	public static float[,] OctavePerlinNoiseMap(int mapWidth, int mapHeight, float scale, float offsetX, float offsetY, float minHeight, float maxHeight, 
		int octaves = 4, float persistence = 0.5f, float lacunarity = 2.0f)
	{
		if (scale <= 0)
			scale = 0.001f;

		float[,] noiseMap = new float[mapWidth, mapHeight];
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				float amplitude = 1.0f;//振幅
				float frequency = 1.0f;//频率
				float noiseHeight = 0f;//累积噪声值

				for (int i = 0; i < octaves; i++)
				{
					float xCoord = (x + offsetX) / scale * frequency;
					float yCoord = (y + offsetY) / scale * frequency;
					float noiseValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
					noiseHeight += noiseValue * amplitude;
					amplitude *= persistence;
					frequency *= lacunarity;
				}
				float normalizedHeight = Mathf.InverseLerp(-1f, 1f, noiseHeight);
				float heightValue = Mathf.Lerp(minHeight, maxHeight, normalizedHeight);
				noiseMap[x, y] = heightValue;
			}
		}
		return noiseMap;
	}

	public static float[,] OctavePerlinNoiseMap(int mapWidth, int mapHeight, float scale, float offsetX, float offsetY, float minHeight, float maxHeight, AnimationCurve heightCurve,
		int octaves = 4, float persistence = 0.5f, float lacunarity = 2.0f)
	{
		if (scale <= 0)
			scale = 0.001f;

		float[,] noiseMap = new float[mapWidth, mapHeight];
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				float amplitude = 1.0f;//振幅
				float frequency = 1.0f;//频率
				float noiseHeight = 0f;//累积噪声值

				for (int i = 0; i < octaves; i++)
				{
					float xCoord = (x + offsetX) / scale * frequency;
					float yCoord = (y + offsetY) / scale * frequency;
					float noiseValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
					noiseHeight += noiseValue * amplitude;
					amplitude *= persistence;
					frequency *= lacunarity;
				}
				float normalizedNoise = Mathf.InverseLerp(-1f, 1f, noiseHeight);
				float curveValue = heightCurve.Evaluate(normalizedNoise);
				float heightValue = Mathf.Lerp(minHeight, maxHeight, curveValue);
				noiseMap[x, y] = heightValue;
			}
		}
		return noiseMap;
	}
}
