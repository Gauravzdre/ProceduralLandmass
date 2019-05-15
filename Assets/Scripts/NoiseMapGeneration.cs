
using System;
using UnityEngine;

public class NoiseMapGeneration {

	public void GenerateNoiseMap(int mapDepth, int mapWidth, float scale, float xOffset, float zOffset, Wave[] waves, Action<float[,]> noiseMapCallback) {

		float[,] noiseMap = new float[mapDepth, mapWidth];

		for (int z = 0; z < mapDepth; z++) {
			for (int x = 0; x < mapWidth; x++) {
				float sampleX = (x + xOffset) / scale;
				float sampleZ = (z + zOffset) / scale;

				float noise = 0;
				float normalization = 0;

				foreach(var wave in waves) {
					noise += wave.amplitude * Mathf.PerlinNoise(sampleX * wave.frequency + wave.seed, sampleZ * wave.frequency + wave.seed);
					normalization += wave.amplitude;
				}

				noise /= normalization;

				noiseMap[z, x] = noise;
				noiseMapCallback(noiseMap);
			}
		}
	}
}

[System.Serializable]
public class Wave {
	public float seed;
	public float frequency;
	public float amplitude;
}