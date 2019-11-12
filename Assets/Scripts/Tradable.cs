using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tradable", menuName = "Tradable", order = 1)]
public class Tradable : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public float PricePerTon;
    public float ProductionRate;
    public float ConsumedPerPerson;
}
