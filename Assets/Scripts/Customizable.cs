using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CustomizableType {Hull, Cargo, Propulsion, Passive }

[CreateAssetMenu(fileName = "Customizable", menuName ="Customizable", order = 1)]
public class Customizable : ScriptableObject
{
    public string Name;
    [TextArea(1,10)]
    public string Description;
    public CustomizableType Type;
    public Sprite ShopIcon;
    public Sprite ShopPreview;
    public Sprite TechIcon;
    public Sprite GameSprite;
    public AudioClip MoveClip;
    public float SuccessRateChange;
    public float SpeedChange;
    public float MaxLoadChange;
    public float RewardMultiplier;
    public bool Researched;
    public bool Unlocked;
    public int Price;
}
