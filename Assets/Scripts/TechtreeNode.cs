using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechtreeNode : MonoBehaviour
{
    public Customizable Customizable;
    public TechtreeNode[] MustHaveResearched;
    public int UnlockTime;
    public int ResearchCost;

    public bool unlocked = false;
    public bool Researched = false;
    public float ConnectionWidth;
    public Color ConnectionColor;
    public Color ResearchedColor;
    public Sprite Default;
    public Sprite Hover;
    public Sprite Selected;

    TechTreeController TechTreeController { get { return GameObject.Find("TechTree").GetComponent<TechTreeController>(); } }
    GameController GameController { get { return GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>(); } }
    Timer Timer { get { return GameObject.Find("Timer").GetComponent<Timer>(); } }
    SpriteRenderer SpriteRenderer { get { return GetComponent<SpriteRenderer>(); } }
    LineRenderer[] LineRenderers;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Customizable.TechIcon;
        LineRenderers = new LineRenderer[MustHaveResearched.Length];
        Timer.YearChange += UpdateUnlock;
        CreateTree();
    }

    private void CreateTree()
    {
        for (int i = 0; i < MustHaveResearched.Length; i++)
        {
            GameObject go = new GameObject("Connection");
            go.transform.parent = transform;
            LineRenderer line = go.AddComponent<LineRenderer>();
            LineRenderers[i] = line;
            Vector3 posDelta = transform.position - MustHaveResearched[i].transform.position;
            Vector3[] positions = { transform.position, new Vector3(transform.position.x - posDelta.x/2, transform.position.y),
            new Vector3(transform.position.x - posDelta.x/2, transform.position.y - posDelta.y),
            new Vector3(transform.position.x - posDelta.x, transform.position.y - posDelta.y) };
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.alignment = LineAlignment.TransformZ;
            line.positionCount = positions.Length;
            line.SetPositions(positions);
            line.sortingOrder = 0;
            line.startWidth = ConnectionWidth;
            line.endWidth = ConnectionWidth;
            LineInit(i, line, this);
        }
    }

    private void LineInit(int i, LineRenderer line, TechtreeNode node)
    {
        if (node.MustHaveResearched[i].Researched)
        {
            line.startColor = node.ResearchedColor;
            line.endColor = node.ResearchedColor;
        }
        else
        {
            line.startColor = node.ConnectionColor;
            line.endColor = node.ConnectionColor;
            line.sortingOrder = 1;
        }
    }

    public void UpdateLines(TechtreeNode node)
    {
        for (int i = 0; i < node.MustHaveResearched.Length; i++)
        {
            LineInit(i, node.LineRenderers[i], node);
        }
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < MustHaveResearched.Length; i++)
        {
            if (MustHaveResearched[i] != null)
            {
                Vector3 posDelta = transform.position - MustHaveResearched[i].transform.position;
                Vector3[] positions = { transform.position, new Vector3(transform.position.x - posDelta.x/2, transform.position.y),
                new Vector3(transform.position.x - posDelta.x/2, transform.position.y - posDelta.y),
                new Vector3(transform.position.x - posDelta.x, transform.position.y - posDelta.y) };
                for (int j = 1; j < positions.Length; j++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(positions[j - 1], positions[j]);
                }
            }
        }
    }

    private void UpdateUnlock(int year)
    {
        if(year > UnlockTime && !unlocked)
        {
            TechTreeController.Unlocked(this);
            unlocked = true;
            Customizable.Unlocked = true;
        }
    }

    public void research()
    {
        Researched = true;
        Customizable.Researched = true;
        GameController.setMoney(-ResearchCost);
        foreach (var node in TechTreeController.nodes)
        {
            UpdateLines(node);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        TechTreeController.Select(this);
        SpriteRenderer.sprite = Selected;
    }
    void OnMouseEnter()
    {
        SpriteRenderer.sprite = Hover;
    }
    void OnMouseExit()
    {
        SpriteRenderer.sprite = Default;
    }
}
