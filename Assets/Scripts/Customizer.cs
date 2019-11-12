using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customizer : MonoBehaviour
{
    public Button NextButton;
    public Button PreviousButton;
    public Image Icon;
    public Text NameText;
    public Text DescriptionText;
    public delegate void ValueChange(Customizable value);
    public event ValueChange Onchange;
    [HideInInspector]
    public Customizable selected { get; private set; }
    [HideInInspector]
    public List<Customizable> customizables;
    public int value = 0;

    public void SetValue(Customizable customizable)
    {
        value = customizables.IndexOf(customizable);
        ChangeValue(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        NextButton.onClick.AddListener(() => ChangeValue(1));
        PreviousButton.onClick.AddListener(() => ChangeValue(-1));
        selected = customizables[value];
        Icon.sprite = selected.ShopIcon;
        NameText.text = selected.Name;
        DescriptionText.text = selected.Description;
    }

    private void ChangeValue(int Direction)
    {
        if (customizables.Count > 1)
        {
            value += Direction;
            if (value >= customizables.Count) value = 0;
            else if (value < 0) value = customizables.Count - 1;
            selected = customizables[value];
            Icon.sprite = selected.ShopIcon;
            NameText.text = selected.Name;
            DescriptionText.text = selected.Description;
            Onchange(selected);
        }
    }
}
