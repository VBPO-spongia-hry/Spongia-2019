using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Pathfinding;
using UnityEngine.UI;

public enum GameState { Map, Shop, TechTree}
public class GameController : MonoBehaviour
{
    public GameState GameState;
    public Camera ShopCamera;
    public Camera MainCamera;
    public Camera TechCamera;
    [Space(10)]
    public GameObject GameUI;
    public GameObject ShopUI;
    public GameObject PathChooseUI;
    public GameObject TechUI;
    public GameObject PauseUI;
    [Space(10)]
    public Button ConfirmPathButton;
    public Button SellButton;
    public Button CancelButton;
    [Space(10)]
    public AstarPath Pathfinder;
    public int shipCount;
    public GameObject ship;
    public List<Ship> ships;
    public Text[] CashTexts;
    public static bool Selecting = false;
    public Color IslandColor;
    public AudioSource AudioSource;
    [Space(10)]
    public AudioClip[] audioClips;
    private CameraController CameraController { get { return Camera.main.GetComponent<CameraController>(); } }
    public MapGenerator MapGenerator { get { return GetComponent<MapGenerator>(); } }
    public int Money;
    private Ship Selected;
    bool paused;
    // Start is called before the first frame update
    void Start()
    {
        Money = 1000;
        ships = new List<Ship>();
        setMoney(0);
        ConfirmPathButton.onClick.AddListener(()=>CanConfirm = true);
        CancelButton.onClick.AddListener(() => Canceled = true);
        SellButton.onClick.AddListener(sell);
        ShowMap();
    }

    void Update()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (!audioSource.isPlaying)
        {
            audioSource.clip = audioClips[Random.Range(0,audioClips.Length)];
            audioSource.Play();
        }
        if(GameState != GameState.Map || paused)
        {
            foreach (var ship in ships)
            {
                if(ship != null)
                    ship.GetComponent<AIPath>().canMove = false;
            }
        }
        else
        {
            foreach (var ship in ships)
            {
                if(ship != null)
                    ship.GetComponent<AIPath>().canMove = true;
            }
        }
    }

    void sell()
    {
        setMoney(Selected.ShipTemplate.CalculatePrice() / 2);
        ships.Remove(Selected);
        Destroy(Selected.gameObject);
        Selected = null;
    }

    public void setMoney(int value)
    {
        Money += value;
        foreach (var CashText in CashTexts)
        {
            CashText.text = Money.ToString();
        }
    }

    public void ShowShop()
    {
        GameState = GameState.Shop;
        AudioSource.Play();
        ShopUI.SetActive(true);
        GameUI.SetActive(false);
        TechUI.SetActive(false);
        TechCamera.tag = "Untagged";
        ShopCamera.tag = "MainCamera";
        MainCamera.tag = "Untagged";
        TechCamera.gameObject.SetActive(false);
        ShopCamera.gameObject.SetActive(true);
        MainCamera.gameObject.SetActive(false);
        foreach (var island in MapGenerator.islands)
        {
            island.CloseWindow();
        }   
    }

    public void ShowMap()
    {
        GameState = GameState.Map;
        if(Camera.main != MainCamera) AudioSource.Play();
        ShopUI.SetActive(false);
        GameUI.SetActive(true);
        TechUI.SetActive(false);
        ShopCamera.tag = "Untagged";
        MainCamera.tag = "MainCamera";
        TechCamera.tag = "Untagged";
        ShopCamera.gameObject.SetActive(false);
        MainCamera.gameObject.SetActive(true);
        TechCamera.gameObject.SetActive(false);
        foreach (var island in MapGenerator.islands)
        {
            island.CloseWindow();
        }
    }

    public void ShowTechTree()
    {
        GameState = GameState.TechTree;
        AudioSource.Play();
        ShopUI.SetActive(false);
        GameUI.SetActive(false);
        TechUI.SetActive(true);
        ShopCamera.tag = "Untagged";
        TechCamera.tag = "MainCamera";
        MainCamera.tag = "Untagged";
        ShopCamera.gameObject.SetActive(false);
        TechCamera.gameObject.SetActive(true);
        MainCamera.gameObject.SetActive(false);
        TechCamera.Render();
        foreach (var island in MapGenerator.islands)
        {
            island.CloseWindow();
        }
    }

    bool Canceled;
    bool CanConfirm;
    public IEnumerator SetRoute(Ship ship)
    {
        foreach (var island in MapGenerator.islands)
        {
            island.CloseWindow();
        }
        foreach (var s in ships)
        {
            s.Canvas.gameObject.SetActive(false);
        }
        Camera.main.GetComponent<CameraController>().enabled = false;
        Selected = ship;
        if (ship.path.Length == 0)
        {
            ships.Add(ship);
            CancelButton.gameObject.SetActive(false);
        }
        else
        {
            CancelButton.gameObject.SetActive(true);
        }
        Vector3 camerapos = Camera.main.transform.position;
        float camerasize = Camera.main.orthographicSize;
        CanConfirm = false;
        Canceled = false;
        PathChooseUI.SetActive(true);
        GameUI.SetActive(false);
        CameraController.EnableMovement = false;
        Camera.main.orthographicSize = 150;
        Camera.main.transform.position = new Vector3(0,0,-10);
        Selecting = true;
        foreach (var island in MapGenerator.islands)
        {
            Color color = island.Color;
            color.a = .5f;
            island.Color = color;
            island.selected = false;
        }
        List<Island> path = new List<Island>();
        yield return new WaitUntil(() => pathSelected(out path));
        if (Selected != null && !Canceled)
        {
            ship.OnPathAssigned(path.ToArray());
        }
        foreach (var island in MapGenerator.islands)
        {
            island.Color = IslandColor;
        }
        Debug.Log(ship.path.Length);
        Camera.main.orthographicSize = camerasize;
        Camera.main.transform.position = camerapos;
        CameraController.EnableMovement = true;
        Selecting = false;
        PathChooseUI.SetActive(false);
        GameUI.SetActive(true);
        Camera.main.GetComponent<CameraController>().enabled = true;
        Selected = null;
    }


    public bool pathSelected(out List<Island> path)
    {
        List<Island> tempPath = new List<Island>();
        int selected = 0;
        foreach (var item in MapGenerator.islands)
        {
            if (item.selected)
            { 
                selected++;
                tempPath.Add(item);
            }
        }
        path = tempPath;
        if (selected < 2 && CanConfirm) CanConfirm = false;
        return (selected >= 2) && CanConfirm || Selected == null || Canceled;
    }

    public void Pause()
    {
        Debug.Log(ships.Count);
        Timer timer = GameObject.Find("Timer").GetComponent<Timer>();
        timer.paused = true;
        paused = true;
        Camera.main.GetComponent<CameraController>().EnableMovement = false;
        PauseUI.SetActive(true);
        foreach (var ship in ships)
        {
           ship.GetComponent<AIPath>().canMove = false;
        }
    }

    public void UnPause()
    {
        Timer timer = GameObject.Find("Timer").GetComponent<Timer>();
        timer.paused = false;
        paused = false;
        PauseUI.SetActive(false);
        Camera.main.GetComponent<CameraController>().EnableMovement = true;
        foreach (var ship in ships)
        {
                ship.GetComponent<AIPath>().canMove = true;
        }
    }

    public void BackToMenu()
    {
        Destroy(GameObject.Find("Timer"));
        SceneManager.LoadScene("Menu");
    }
}
