using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Timer : MonoBehaviour
{
    public delegate void TimeChange(int year);
    public event TimeChange YearChange;
    public Text TimeText;
    public float timescale;
    public int MaxTurnsCount;
    public static int[] Money;
    [HideInInspector]
    public bool paused;
    [Space(10)]
    [Tooltip("Values for graph plotter")]
    public float GraphWidth;
    public float GraphHeight;
    public int GraphResolution;
    float startX = -6.5f;
    float endX = 3;

    LineRenderer LineRenderer { get { return GameObject.Find("Plotter").GetComponent<LineRenderer>(); } }
    GameController GameController { get { return GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>(); } }
    int _time;
    int time
    {
        get
        {
            return _time;
        }
        set
        {
            _time = value;
            TimeText.text = value.ToString();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Money = new int[MaxTurnsCount];
        YearChange += Timer_YearChange;
        StartCoroutine(MainLoop());
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("End"))
        {
            int max = Mathf.Max(Money);
            GraphResolution = Mathf.Min(GraphResolution, Money.Length);
            List<Vector3> points = new List<Vector3>();
            int index = 0;
            for (float i = 0; i < GraphResolution; i+= Money.Length / (float)GraphResolution)
            {
                float NormalizedValue = Mathf.InverseLerp(0, max, Money[index]);
                points.Add(new Vector3(i*(endX - startX) / GraphResolution + startX, Mathf.Lerp(-4, 1, NormalizedValue),-1));
                index++;
            }
            Text MaxText = GameObject.Find("MaxText").GetComponent<Text>();
            MaxText.text = "Max money: " + max;
            LineRenderer.positionCount = points.Count;
            LineRenderer.SetPositions(points.ToArray());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Timer_YearChange(int year)
    {
        
    }

    IEnumerator MainLoop()
    {
        time = 0;
        int i = 0;
        if (GameCreator.infinite)
        {
            while (true)
            {
                //tu mozeme nejako menit timescale
                time++;
                YearChange(time);
                i++;
                yield return new WaitForSeconds(timescale);
                while (paused)
                    yield return null;
                while (GameController.GameState != GameState.Map)
                    yield return null;
            }
        }
        else
        {
            while (i < MaxTurnsCount)
            {
                Money[i] = GameController.Money;
                //tu mozeme nejako menit timescale
                time++;
                YearChange(time);
                i++;
                yield return new WaitForSeconds(timescale);
                while (paused)
                    yield return null;
                while (GameController.GameState != GameState.Map)
                    yield return null;
            }
        }
        SceneManager.LoadScene("End");
    }
}
