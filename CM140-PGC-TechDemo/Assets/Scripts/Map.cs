using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MapDisplay
{
    Heat,
    Biome,
    Height,
    Moisture

}

[ExecuteInEditMode]
public class Map : MonoBehaviour
{

  

    public BiomePreset[] biomes;
    public GameObject tilePrefab;
    public MapDisplay displayType;
    public RawImage debugImage;
    [SerializeField]
    private int numOfEnemies;
    public GameObject prefab;


    [Header("Dimensions")]
    public int height;
    public int width;
    public float scale;
    public Vector2 offset;

    [Header("Moisture Map")]
    public Wave[] moistureWaves;
    public Gradient moistureDebugColors;
    public float[,] moistureMap;

    [Header("Heat Map")]
    public Wave[] heatWaves;
    public Gradient heatDebugColors;
    public float[,] heatMap;

    [Header("Height Map")]
    public Wave[] heightWaves;
    public Gradient heightDebugColors;
    public float[,] heightMap;





    private float lastGenerateTime;

    void Start ()
    {
        if(Application.isPlaying)
            CreateMap();
    }
    
    void Update ()
    {
        if(Application.isPlaying)
            return;

        // true every 0.1 seconds
        if(Time.time - lastGenerateTime > 0.1f)
        {
            lastGenerateTime = Time.time;
            CreateMap();
        }
    }

    void CreateMap ()
    {
        // Create the height map
        heightMap = NoiseGenerator.Generate(width, height, scale, offset, heightWaves);

        // Create the moisture map
        moistureMap = NoiseGenerator.Generate(width, height, scale, offset, moistureWaves);

        // Create the heat map
        heatMap = NoiseGenerator.Generate(width, height, scale, offset, heatWaves);


        //creates an array for each of the pixels in the texture
        Color[] pixels = new Color[width * height];
        int i = 0;


        // loop through each pixel
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {


                // how do we want to display the debug map?
                switch (displayType)
                {
                    case MapDisplay.Height:
                        pixels[i] = heightDebugColors.Evaluate(heightMap[x, y]);
                        break;
                    case MapDisplay.Moisture:
                        pixels[i] = moistureDebugColors.Evaluate(moistureMap[x, y]);
                        break;
                    case MapDisplay.Heat:
                        pixels[i] = heatDebugColors.Evaluate(heatMap[x, y]);
                        break;
                    case MapDisplay.Biome:
                    {
                        BiomePreset biome = GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y]);
                        pixels[i] = biome.debugColor;

                        if(Application.isPlaying)
                        {
                            
                            GameObject tile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                            tile.GetComponent<SpriteRenderer>().sprite = biome.GetTileSprite();
                                tile.GetComponent<BoxCollider2D>();


                        }

                        break;
                    }
                }

                i++;
            }
        }



        // create the texture
        Texture2D tex = new Texture2D(width, height);
        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        // apply the texture to the raw image
        debugImage.texture = tex;

        SpawnEnemies();
    }

    // will get the biome that is closest to what the noise maps gives
    BiomePreset GetBiome (float height, float moisture, float heat)
    {
        BiomePreset biomeToReturn = null;
        List<BiomePreset> tempBiomes = new List<BiomePreset>();

        // will loop through each of the biomes if it  matches the requirements it will be added to tempbiomes
        foreach(BiomePreset biome in biomes)
        {
            if(biome.MatchCondition(height, moisture, heat))
            {
                tempBiomes.Add(biome);
            }
        }

        float curVal = 0.0f;

        // loop through each of the biomes that meet the  requirements
        // find the one closes to the original height, moisture and heat values
        foreach(BiomePreset biome in tempBiomes)
        {
            float diffVal = (height - biome.minHeight) + (moisture - biome.minMoisture) + (heat - biome.minHeat);

            if(biomeToReturn == null)
            {
                biomeToReturn = biome;
                curVal = diffVal;
            }
            else if(diffVal < curVal)
            {
                biomeToReturn = biome;
                curVal = diffVal;
            }
        }

        // if no biome is found - return the first one in the biomes array
        if(biomeToReturn == null)
            return biomes[0];

        return biomeToReturn;
    }
    public void SpawnEnemies()
    {
        for (int j = 0; j < numOfEnemies; j++)
        {
            if (Application.isPlaying)
            {

                Vector3 position = new Vector3(Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));
                Instantiate(prefab, position, Quaternion.identity);
            }
        }
    }
}