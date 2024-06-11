using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;


public enum NoiseType
{
    Perlin,
    Simplex,
    Cellular//also know as Voronoi/Worley 
}

public static class Noice
{
    public static float[,] GenerateNoiseMap(NoiseType noiseType, int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x * frequency;
                    float sampleY = (y - halfHeight) / scale * frequency - octaveOffsets[i].y * frequency;

                    float perlinValue = 0;

                    switch (noiseType)
                    {
                        case NoiseType.Perlin:
                            perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                            break;
                        case NoiseType.Simplex:
                            perlinValue = Unity.Mathematics.noise.snoise(new float2(sampleX,sampleY)) *2 - 1;
                            break;
                        case NoiseType.Cellular:
                            float2 pos = new float2(sampleX, sampleY);
                            var cellularResult = noise.cellular(pos);
                            perlinValue = cellularResult.x * 2 - 1; // Use F1 value

                            break;
                        default:
                            Debug.LogError("Unknown noise type.");
                            break;
                    }

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;

        
    }   
}
