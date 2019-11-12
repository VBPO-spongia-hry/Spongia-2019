using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradableInfo : MonoBehaviour
{
    public Tradable Tradable;
    public Slider Slider;
    public Image icon;
    public Text NameText;
    public Text StorageText;
    public Text NeededText;
    public Text ExcessText;
    public Island island;
    public float Amount;
    public float Needed;
    public Gradient Gradient;

    // Start is called before the first frame update
    void Start()
    {
        NameText.text = Tradable.Name;
        Slider.interactable = false;
        icon.sprite = Tradable.Icon;
        updateSlider();
    }

    public void NextYear(int year)
    {
        Amount += Tradable.ProductionRate;
        updateSlider();
    }

    public void updateSlider()
    {
        StorageText.text = "Have: " + Amount + " T";
        NeededText.text = "Need: " + Needed + " T";
        ExcessText.text = "Excess: " + (Amount - Needed) + " T";
        if (Needed == 0) return;
        Slider.maxValue = 2 * Needed;
        Slider.minValue = 0;
        Slider.value = Mathf.Clamp(Amount, 0, 2 * Needed);
        Slider.fillRect.GetComponent<Image>().color = Gradient.Evaluate(Mathf.InverseLerp(0, 2 * Needed, Slider.value));
    }
}
