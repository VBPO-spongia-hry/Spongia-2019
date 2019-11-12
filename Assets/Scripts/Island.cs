using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    #region premenne
    public int IslandWidth;
    public int IslandHeight;

    /*public float noiseScale;
    [Range(0,1)]
    public float NoiseThreshold;
    public Color IslandColor;
    public Color WaterColor;
    public Vector2 offset;
    public float colliderResolution;*/

    public GameObject[] IslandTemplates;
    [HideInInspector]
    public int seed;
    public float ChunkSize;
    public bool selected;
    [Space(20)]
    public Tradable[] tradables;
    public RectTransform Info;
    public GameObject TradableInfo;
    public TradableInfo[] tradableInfos;
    public Animation Animation;

    #endregion
    GameController GameController { get { return GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>(); } }
    AudioSource AudioSource { get { return GetComponent<AudioSource>(); } }
    public Canvas Canvas { get { return transform.GetChild(0).GetComponent<Canvas>(); } }
    IslandTemplate[] templates;
    public Color Color
    {
        get
        {
            return transform.GetChild(1).GetComponent<SpriteRenderer>().color;
        }
        set
        {
            for (int i = 0; i < 4; i++)
            {
                transform.GetChild(i + 1).GetComponent<SpriteRenderer>().color = value;
            }
        }
    }
    /*SpriteRenderer SpriteRenderer { get { return GetComponent<SpriteRenderer>(); } }
    PolygonCollider2D PolygonCollider { get { return GetComponent<PolygonCollider2D>(); } }*/
    // Start is called before the first frame update
    void Start()
    {
        /*float[,] heights = GenerateNoiseMap();
        Color[] colors = GetColors(heights);
        //PolygonCollider.points = SetCollider(heights);
        Texture2D texture = new Texture2D(IslandWidth, IslandHeight);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colors);
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, IslandWidth, IslandHeight), new Vector2(.5f, .5f));
        SpriteRenderer.sprite = sprite;*/
        System.Random prng = new System.Random(seed);
        Generate(prng);
    }

    private void Generate(System.Random prng)
    {
        Timer timer = GameObject.Find("Timer").GetComponent<Timer>();
        timer.YearChange += Timer_YearChange;
        tradableInfos = new TradableInfo[tradables.Length];
        templates = new IslandTemplate[4];
        for (int i = 0; i < tradables.Length; i++)
        {
            tradableInfos[i] = Instantiate(TradableInfo, Info).GetComponent<TradableInfo>();
            tradableInfos[i].Tradable = tradables[i];
        }
        for (int i = 0; i < 4; i++)
        {
            GameObject go = Instantiate(IslandTemplates[prng.Next(IslandTemplates.Length)], transform);
            go.transform.rotation = Quaternion.Euler(0, 0, 90 * i);
            int index = prng.Next(tradables.Length);
            templates[i] = go.GetComponent<IslandTemplate>();
            go.GetComponent<IslandTemplate>().tradable = tradables[index];
            go.GetComponent<IslandTemplate>().TradableInfo = tradableInfos[index];
        }
        transform.localScale = new Vector3(IslandHeight, IslandWidth);
        transform.rotation = Quaternion.Euler(0, 0, prng.Next(360));
        Color = GameController.IslandColor;
    }

    private void Timer_YearChange(int year)
    {
        for (int i = 0; i < tradables.Length; i++)
        {
            int population = 0;
            foreach (var template in templates)
            {
                population += template.Population;
            }
            tradableInfos[i].Needed = tradables[i].ConsumedPerPerson * population;
            tradableInfos[i].updateSlider();
        }
    }
    [HideInInspector]
    bool windowOpen = false;
    public void Select()
    {
        AudioSource.Play();
        if (GameController.Selecting)
        {
            selected = !selected;
            if (selected)
            {
                Color color = Color;
                color.a = 1f;
                Color = color;
            }
            else
            {
                Color color = Color;
                color.a = .5f;
                Color = color;
            }
            windowOpen = false;
        }
        else
        {
            foreach (var island in GameController.GetComponent<MapGenerator>().islands)
            {
                if(island == this)
                {
                    if(!windowOpen) Animation.Play("WindowOpen");
                    windowOpen = true;
                }
                else
                {
                    if(island.windowOpen) island.Animation.Play("WindowClose");
                    island.windowOpen = false;
                }
            }
        }
    }

    public void CloseWindow(bool fromController = true)
    {
        if (fromController)
        {
            if(windowOpen) Animation.Play("WindowClose");
        }
        else
        {
            Animation.Play("WindowClose");
            AudioSource.Play();
        }
        windowOpen = false;
    }


    /* private float[,] GenerateNoiseMap()
     {
         float[,] heightmap = new float[IslandHeight, IslandWidth];
         float MaxDist = Mathf.Sqrt(IslandHeight / 2 * IslandHeight / 2 + IslandWidth / 2 * IslandWidth / 2);
         for (int i = -IslandHeight/2; i < IslandHeight/2; i++)
         {
             for (int j = -IslandWidth/2; j < IslandWidth/2; j++)
             {
                 float noiseVal = Mathf.PerlinNoise(i/noiseScale+offset.y, j/noiseScale+offset.x);
                 float dist = Mathf.Sqrt(i * i + j * j);
                 float normalizeDist = Mathf.InverseLerp(0,MaxDist,dist);
                 heightmap[i + IslandHeight / 2, j + IslandWidth / 2] = Mathf.Clamp01(noiseVal - normalizeDist);
             }
         }
         return heightmap;
     }
     private Color[] GetColors(float[,] heights)
     {
         Color[] colors = new Color[IslandWidth * IslandHeight];
         for (int i = 0; i < heights.GetLength(0); i++)
         {
             for (int j = 0; j < heights.GetLength(1); j++)
             {
                 if(heights[i,j] < NoiseThreshold)
                 {
                     colors[j + i * IslandWidth] = new Color(WaterColor.r, WaterColor.g, WaterColor.b, WaterColor.a);
                 }
                 else
                 {
                     colors[j + i * IslandWidth] = new Color(IslandColor.r, IslandColor.g, IslandColor.b, IslandColor.a);
                 }
                 //colors[j+i*IslandWidth] = new Color(heights[i,j],heights[i,j],heights[i,j]);
             }
         }
         return colors;
     }*/
}