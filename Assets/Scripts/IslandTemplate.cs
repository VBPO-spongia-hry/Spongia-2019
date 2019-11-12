using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandTemplate : MonoBehaviour
{
    public int Population;
    public int PopulationGrowth;
    public int YearsToGrow;
    public Tradable tradable;
    public TradableInfo TradableInfo;

    void Start()
    {
        Timer timer = GameObject.Find("Timer").GetComponent<Timer>();
        timer.YearChange += NextYear;
    }

    private void OnMouseDown()
    {
        Island islandGenerator = transform.parent.gameObject.GetComponent<Island>();
        islandGenerator.Select();
    }

    void NextYear(int year)
    {
        if(year % YearsToGrow == 0)Population += PopulationGrowth;
        TradableInfo.Needed = Population * tradable.ConsumedPerPerson;
        TradableInfo.NextYear(year);
    }
}
