using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechTreeController : MonoBehaviour
{
    public Text InfoText;
    public Text NameText;
    public Text TimeText;
    public Text CanResearchText;
    public Image image;

    public Button ResearchButton;
    public TechtreeNode[] nodes;
    public Text CostText;

    public TechtreeNode Selected;
    GameController GameController { get { return GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>(); } }
    ShopController ShopController { get { return GameObject.Find("Shop").GetComponent<ShopController>(); } }
    AudioSource AudioSource { get { return GetComponent<AudioSource>(); } }
    // Start is called before the first frame update
    void Start()
    {
        ResearchButton.onClick.AddListener(() => research());
        UpdateUI(Selected);
        GameObject[] go = GameObject.FindGameObjectsWithTag("Node");
        nodes = new TechtreeNode[go.Length];
        for (int i = 0; i < go.Length; i++)
        {
            nodes[i] = go[i].GetComponent<TechtreeNode>();
        }
        Selected = nodes[0];
    }

    void research()
    {
        if (!Selected.Researched && GameController.Money >= Selected.ResearchCost)
        {
            Selected.research();
            ShopController.GetCustomizables();
            UpdateUI(Selected);
        }
    }

    public void BackToMap()
    {
        GameController.ShowMap();   
    }
    
    public void Select(TechtreeNode node)
    {
        Selected = node;
        UpdateUI(node);
        AudioSource.Play();
    }

    public void Unlocked(TechtreeNode node)
    {
        if(Selected == node)
        {
            node.unlocked = true;
            UpdateUI(node);
        }
    }

    public void UpdateUI(TechtreeNode node)
    {
        image.sprite = node.Customizable.TechIcon;
        CostText.text = node.ResearchCost.ToString();
        NameText.text = node.Customizable.Name;
        InfoText.text = node.Customizable.Description;
        if (node.Researched)
        {
            TimeText.gameObject.SetActive(true);
            TimeText.text = "This tech has already been researched";
            TimeText.color = Color.green;
            ResearchButton.interactable = false;
            return;
        }
        bool canresearch = true;
        List<TechtreeNode> notResearched = new List<TechtreeNode>();
        foreach (var item in node.MustHaveResearched)
        {
            if (!item.Researched) notResearched.Add(item);
        }
        if (notResearched.Count == 0)
        {
            CanResearchText.gameObject.SetActive(false);
            ResearchButton.interactable = true;
        }
        else if (notResearched.Count == 1)
        {
            CanResearchText.gameObject.SetActive(true);
            ResearchButton.interactable = false;
            CanResearchText.text = "You can't research " + node.Customizable.Name + " because " + notResearched[0].Customizable.Name + " isn't researched.";
            canresearch = false;
        }
        else
        {
            CanResearchText.gameObject.SetActive(true);
            ResearchButton.interactable = false;
            string techs = "";
            for (int i = 0; i < notResearched.Count; i++)
            {
                techs += notResearched[i].Customizable.Name;
                if (i != notResearched.Count - 1) techs += ", ";
            }
            CanResearchText.text = "You can't research " + node.Customizable.Name + " because " + techs + " aren't yet researched.";
            canresearch = false;
        }

        if (!node.unlocked || !canresearch)
        {
            if (!node.unlocked)
            {
                Debug.Log(node.Customizable.Name + " " + node.UnlockTime);
                TimeText.gameObject.SetActive(true);
                TimeText.color = Color.red;
                TimeText.text = "You can research " + node.Customizable.Name + " at time " + node.UnlockTime;
                ResearchButton.interactable = false;
            }
            else
            {
                TimeText.gameObject.SetActive(false);
            }
        }
        else
        {
            if (GameController.Money >= node.ResearchCost)
            {
                TimeText.gameObject.SetActive(false);
                ResearchButton.interactable = true;
            }
            else
            {
                TimeText.gameObject.SetActive(true);
                TimeText.color = Color.red;
                TimeText.text = "You don't have enough money to research this tech";
                ResearchButton.interactable = false;
            }
        }
    }
}