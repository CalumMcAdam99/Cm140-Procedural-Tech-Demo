using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public static float[,] Generate (int width, int height, float scale, Vector2 offset, Wave[] waves)
    {
        // create the noise map
        float[,] noiseMap = new float[width, height];

        // loop through each element in the noise map
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                // calculate the sample positions
                float PosX = (float)x * scale + offset.x;
                float PosY = (float)y * scale + offset.y;

                float normalization = 0.0f;

                // loop through each wave
                foreach(Wave wave in waves)
                {
                    // create a sample position with the wave's seed and frequency included
                    float wavePosX = PosX * wave.frequency + wave.seed;
                    float wavePosY = PosY * wave.frequency + wave.seed;

                    // sample the perlin noise with amplitude
                    noiseMap[x, y] += wave.amplitude * Mathf.PerlinNoise(wavePosX, wavePosY);
                    normalization += wave.amplitude;
                }

                // normalize the value to be within a 0.0 - 1.0 range
                noiseMap[x, y] /= normalization;
            }
        }

        return noiseMap;
    }
}

[System.Serializable]
public class Wave
{
    public float seed;
    public float frequency;
    public float amplitude;
}