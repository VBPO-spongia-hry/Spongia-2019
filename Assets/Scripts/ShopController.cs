using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Pathfinding;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    GameController gameController;
    public GameObject Ship;
    public Dropdown TemplateDropdown;
    public Customizer HullCustomizer;
    public Customizer PropulsionCustomizer;
    public Customizer CargoCustomizer;
    public Customizable[] Customizables;
    public List<ShipTemplate> templates;
    public Button BuyButton;
    ShipTemplate Template;
    public InputField nameField;
    public Text costText;
    public GameObject ShipPreview;
    public AudioSource AudioSource;
    public AudioClip Klik;
    public AudioClip BadKlik;
    List<Customizable> PowerUps;
    // Start is called before the first frame update
    void Start()
    {
        PowerUps = new List<Customizable>();
        foreach (var customizable in Customizables)
        {
            if (customizable.Name == "Paddles" || customizable.Name == "Straw Boat" || customizable.Name == "Barrel")
            {
                customizable.Unlocked = true;
                customizable.Researched = true;
            }
            else
            {
                customizable.Researched = false;
                customizable.Unlocked = false;
            }
        }
        HullCustomizer.Onchange += UpdateTemplate;
        CargoCustomizer.Onchange += UpdateTemplate;
        PropulsionCustomizer.Onchange += UpdateTemplate;
        TemplateDropdown.onValueChanged.AddListener((value) => DropdownChange(value));
        templates = new List<ShipTemplate>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        var customization = GetCustomizables();
        Template = new ShipTemplate(customization.h, customization.p, customization.c, "Default", PowerUps.ToArray());
        UpdateTemplate(customization.p);
        costText.text = Template.CalculatePrice().ToString();
        ShipPreview.transform.Find("Hull").GetComponent<SpriteRenderer>().sprite = customization.h.ShopPreview;
        ShipPreview.transform.Find("Propulsion").GetComponent<SpriteRenderer>().sprite = customization.p.ShopPreview;
        ShipPreview.transform.Find("Cargo").GetComponent<SpriteRenderer>().sprite = customization.c.ShopPreview;
        HullCustomizer.SetValue(customization.h);
        PropulsionCustomizer.SetValue(customization.p);
        CargoCustomizer.SetValue(customization.c);
    }

    public (Customizable h, Customizable p, Customizable c) GetCustomizables()
    {
        Customizable hull = null, propulsion = null, cargo = null;
        for (int i = Customizables.Length - 1; i >= 0; i--)
        {
            Customizable customizable = Customizables[i];
            if (customizable.Researched)
            {
                switch (customizable.Type)
                {
                    case CustomizableType.Hull:
                        if (!HullCustomizer.customizables.Contains(customizable))
                        {
                            HullCustomizer.customizables.Add(customizable);
                            hull = customizable;
                        }
                        break;
                    case CustomizableType.Cargo:
                        if (!CargoCustomizer.customizables.Contains(customizable))
                        {
                            CargoCustomizer.customizables.Add(customizable);
                            cargo = customizable;
                        }
                        break;
                    case CustomizableType.Propulsion:
                        if (!PropulsionCustomizer.customizables.Contains(customizable))
                        {
                            PropulsionCustomizer.customizables.Add(customizable);
                            propulsion = customizable;
                        }
                        break;
                    case CustomizableType.Passive:
                        PowerUps.Add(customizable);
                        break;
                    default:
                        break;
                }
            }
        }
        var customization = (hull, propulsion, cargo);
        return customization;
    }

    // Update is called once per frame
    void UpdateTemplate(Customizable customizable)
    {
        switch (customizable.Type)
        {
            case CustomizableType.Cargo:
                Template.Cargo = customizable;
                ShipPreview.transform.Find("Cargo").GetComponent<SpriteRenderer>().sprite = customizable.ShopPreview;
                break;
            case CustomizableType.Hull:
                Template.Hull = customizable;
                ShipPreview.transform.Find("Hull").GetComponent<SpriteRenderer>().sprite = customizable.ShopPreview;
                break;
            case CustomizableType.Propulsion:
                Template.Propulsion = customizable;
                SpriteRenderer spriteRenderer = ShipPreview.transform.Find("Propulsion").GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = customizable.ShopPreview;
                if (customizable.Name == "Paddles") spriteRenderer.sortingOrder = 3;
                else spriteRenderer.sortingOrder = 1;
                break;
            case CustomizableType.Passive:
                break;
            default:
                break;
        }
        costText.text = Template.CalculatePrice().ToString();
    }

    public void DropdownChange(int value)
    {
        Debug.Log(value);
        Template = templates[value];
        HullCustomizer.SetValue(Template.Hull);
        PropulsionCustomizer.SetValue(Template.Propulsion);
        CargoCustomizer.SetValue(Template.Cargo);
    }

    public void Buy()
    {
        if (gameController.Money >= Template.CalculatePrice())
        {
            string name = "Default";
            foreach (var temp in templates)
            {
                if (temp.Propulsion == Template.Propulsion && temp.Cargo == Template.Cargo && temp.Hull == Template.Hull) name = temp.TemplateName;
            }
            Template.TemplateName = name;
            Ship ship = Instantiate(Ship).GetComponent<Ship>();
            ship.ShipTemplate = Template;
            gameController.setMoney(-Template.CalculatePrice());
            BackToMap();
            AudioSource.clip = Klik;
            AudioSource.Play();
        }
        else
        {
            AudioSource.clip = BadKlik;
            AudioSource.Play();
        }
    }

    public void Save()
    {
        if (nameField.text != string.Empty)
        {
            Debug.Log("Save");
            foreach (var temp in templates)
            {
                if (temp.Propulsion == Template.Propulsion && temp.Cargo == Template.Cargo && temp.Hull == Template.Hull) return;
            }
            Debug.Log(Template);
            Template.TemplateName = nameField.text;
            templates.Add(Template);
            Template = new ShipTemplate(HullCustomizer.selected, PropulsionCustomizer.selected, CargoCustomizer.selected, "", PowerUps.ToArray());
            TemplateDropdown.AddOptions(new List<string>() { nameField.text });
        }
    }

    public void BackToMap()
    {
        gameController.ShowMap();
    }
}
