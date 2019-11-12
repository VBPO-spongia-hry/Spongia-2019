using System.Collections;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    AIPath AIPath { get { return GetComponent<AIPath>(); } }
    public ShipTemplate ShipTemplate;
    public Canvas Canvas;
    public Text NameText;
    //tu je pole ostrovov na ktore sa musime dostat
    public Island[] path;
    public Tradable[] Carrying;
    public SpriteRenderer[] Cargos;
    public Image[] resources;
    public SpriteRenderer Frame;
    public SpriteRenderer Propulsion;
    public AudioClip PathClip;
    public AudioClip ExplodeClip;
    Sprite cargo;
    private GameController GameController { get { return GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>(); } }
    private Vector3 offset;
    private int pathIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AIPath>().Teleport(getWalkablePoint());
        Carrying = new Tradable[6];
        cargo = ShipTemplate.Cargo.GameSprite;
        offset = Canvas.transform.position - transform.position;
        Propulsion.sprite = ShipTemplate.Propulsion.GameSprite;
        if (ShipTemplate.Propulsion.Name == "Paddles" || ShipTemplate.Propulsion.Name == "Sail" || ShipTemplate.Propulsion.Name == "Steam Engine")
            Propulsion.sortingOrder = 1;
        else
            Propulsion.sortingOrder = -1;
        Frame.sprite = ShipTemplate.Hull.GameSprite;
        Canvas.worldCamera = Camera.main;
        NameText.text = ShipTemplate.TemplateName;
        SetPath();
        foreach (var cargo in Cargos)
        {
            cargo.sprite = null;
        }
    }

    Vector3 getWalkablePoint()
    {
        Vector3 point = new Vector3();
        bool suitable = false;
        while (!suitable)
        {
            suitable = true;
            foreach (var island in GameController.MapGenerator.islands)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (island.transform.GetChild(i+1).GetComponent<PolygonCollider2D>().bounds.Contains(point))
                    {
                        suitable = false;
                        break;
                    }
                }
                if (!suitable) break;
            }
            if (!suitable)
            {
                point.x++;
                point.y++;
            }
        }
        return point;
    }

    public void OnPathAssigned(Island[] p)
    {
        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().clip = PathClip;
        GetComponent<AudioSource>().Play();
        AIPath.maxSpeed = ShipTemplate.CalculateSpeed();
        AIPath.destination = p[0].transform.position;
        path = p;
        pathIndex = 0;
        Canvas.gameObject.SetActive(false);
    }

    Island lastIsland = null;
    bool hasDestination = false;
    // Update is called once per frame
    void Update()
    {
        if (path.Length > 0)
        {
            if (AIPath.reachedEndOfPath)
            {
                lastIsland = path[pathIndex];
                if (!hasDestination)
                {
                    ReachedIsland();
                    pathIndex++;
                    if (pathIndex > path.Length - 1) pathIndex = 0;
                    AIPath.destination = path[pathIndex].transform.position;
                    hasDestination = true;
                    StartCoroutine(Wait());
                }
            }
        }
        Canvas.transform.localPosition = transform.InverseTransformPoint(transform.position + Vector3.up * offset.magnitude);
        Canvas.transform.forward = Camera.main.transform.forward;
        Vector3 pos = Canvas.transform.position;
        pos.z = -3;
        Canvas.transform.position = pos;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(.5f);
        hasDestination = false;
    }

    private void ReachedIsland()
    {
        //Vylozime naklad
        int index = 0;
        foreach (var tradable in Carrying)
        {
            if (tradable == null) continue;
            foreach(var t in path[pathIndex].tradableInfos)
            {
                if(t.Tradable == tradable)
                {
                    if(t.Amount < t.Needed)
                    {
                        t.Amount += ShipTemplate.CalculateCargo();
                        t.updateSlider();
                        Debug.Log("Delivered " + tradable.Name + ": " + ShipTemplate.CalculateCargo().ToString());
                        GameController.setMoney((int)(ShipTemplate.CalculateCargo() * tradable.PricePerTon));
                        Cargos[index].sprite = null;
                        resources[index].sprite = null;
                        Carrying[index] = null;
                        index++;
                        break;
                    }
                }
            }
        }
        //Nalozime naklad
        int tradableIndex = 0;
        foreach (var info in path[pathIndex].tradableInfos)
        {
            if (IsNeededAtRoute(info.Tradable))
            {
                int i = 0;
                while (info.Amount - ShipTemplate.CalculateCargo() > info.Needed)
                {
                    if (tradableIndex == Carrying.Length)
                    {
                        break;
                    }
                    if (Carrying[tradableIndex] == null) Carrying[tradableIndex] = info.Tradable;
                    else
                    {
                        tradableIndex++;
                        continue;
                    }
                    resources[tradableIndex].sprite = info.Tradable.Icon;
                    Cargos[tradableIndex].sprite = cargo;
                    tradableIndex++;
                    i += ShipTemplate.CalculateCargo();
                    info.Amount -= ShipTemplate.CalculateCargo();
                    info.updateSlider();
                }
                Debug.Log("Sent " + info.Tradable.Name + ": " + i.ToString());
                Debug.Log(info.Amount - info.Needed);
            }
        }
        if (100*Random.value > ShipTemplate.CalculateSuccessRate())
        {
            StartCoroutine(Sink());
        }
    }

    public bool IsNeededAtRoute(Tradable tradable)
    {
        bool need = false;
        foreach (var island in path)
        {
            foreach (var info in island.tradableInfos)
            {
                if(info.Tradable == tradable)
                {
                    if(info.Amount < info.Needed)
                    {
                        need = true;
                        return need;
                    }
                }
            }
        }
        return need;
    }
    IEnumerator Sink()
    {
        yield return new WaitForSeconds(Random.Range(2,8));
        AudioSource audioSource = GameObject.Find("Explode").GetComponent<AudioSource>();
        audioSource.Play();
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        foreach (var ship in GameController.ships)
        {
            ship.Canvas.gameObject.SetActive(false);
        }
        Canvas.gameObject.SetActive(true);
        Camera.main.GetComponent<CameraController>().follow(this);
    }

    public void SetPath()
    {
        GameController.StartCoroutine(GameController.SetRoute(this));
    }
}

public class ShipTemplate
{
    public Customizable Hull;
    public Customizable Propulsion;
    public Customizable Cargo;
    public string TemplateName;
    public static Customizable[] PowerUps;

    public ShipTemplate(Customizable hull, Customizable propulsion, Customizable cargo, string templateName, Customizable[] powerUps)
    {
        Hull = hull;
        Propulsion = propulsion;
        Cargo = cargo;
        TemplateName = templateName;
        PowerUps = powerUps;
    }

    public float CalculateSpeed()
    {
        //funkcia na zistenie celkovej rychlosti lode z danej konfiguracie
        float y;
        y = ((Propulsion.SpeedChange * 2) / 10) * (10 - (Cargo.SpeedChange - Hull.SpeedChange));
        return y;
    }

    public float CalculateSuccessRate()
    {
        //funkcia na zistenie celkoveho success rate z danej konfiguracie
        float x=Hull.SuccessRateChange+Propulsion.SuccessRateChange;
        foreach (var power in PowerUps)
        {
            x += power.SuccessRateChange;
        }
        float y=60;

        for (int j=1; j<x+1; j++)
            y=y+(40/2^j);
        return y;
    }

    public int CalculatePrice()
    {
        //zistenie ceny konfiguracie
        return Hull.Price + Propulsion.Price + Cargo.Price;
    }

    public int CalculateCargo()
    {
        float cargo = Cargo.MaxLoadChange + Hull.MaxLoadChange / 6 + Propulsion.MaxLoadChange / 6;
        foreach (var item in PowerUps)
        {
            cargo *= (100 + item.MaxLoadChange) / 100;   
        }
        return (int)cargo;
    }
}
