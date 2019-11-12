using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int Seed;
    public int IslandCount;
    public GameObject Island;
    public int MapWidth;
    public int MapHeight;
    public float MinDistance;
    public List<Island> islands;
    public int ChooseCount;
    public int MinIslandSize;
    public int MaxIslandSize;
    public AstarPath AstarPath;

    GameController GameController { get { return GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>(); } }
    // Start is called before the first frame update
    void Start()
    {
        Seed = GameCreator.seed;
        System.Random prng = new System.Random(Seed);
        islands = new List<Island>();
        int seed = Seed;
        for (int i = 0; i < IslandCount; i++)
        {
            float minDist = 0;
            Vector3 pos = new Vector3();
            int j = 0;
            while (minDist < MinDistance)
            {
                minDist = float.MaxValue;
                pos = new Vector3(prng.Next(-MapWidth/2, MapWidth/2), prng.Next(-MapHeight/2, MapHeight/2));
                foreach (var island in islands)
                {
                    float dist = Vector3.Distance(pos, island.transform.position);
                    if (dist < minDist)
                        minDist = dist;
                }
                if (islands.Count == 0) minDist = float.MaxValue;
                j++;
                if (j > ChooseCount) break;
            }
            if (minDist < MinDistance && islands.Count > 0) break;
            Island islandGenerator = Instantiate(Island, pos, Quaternion.identity, transform).GetComponent<Island>();
            int size = prng.Next(MinIslandSize, MaxIslandSize);
            islandGenerator.IslandHeight = size;
            islandGenerator.IslandWidth = size;
            islandGenerator.seed = seed;
            islands.Add(islandGenerator);
            seed++;
        }
        StartCoroutine(Scan());
    }

    IEnumerator Scan()
    {
        yield return new WaitForSeconds(.3f);
        AstarPath.Scan();
    }
}
