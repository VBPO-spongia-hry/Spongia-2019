using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCreator : MonoBehaviour
{
    public Toggle infiniteToggle;
    public InputField SeedField;

    public static bool infinite;
    public static int seed;
    void Start()
    {
        //DontDestroyOnLoad(gameObject);
        SeedField.text = Random.Range(0, 65536).ToString();
    }

    public void CreateGame()
    {
        infinite = infiniteToggle.isOn;
        if(SeedField.text != string.Empty)
        {
            if (int.TryParse(SeedField.text,out seed)) seed = int.Parse(SeedField.text);
            else
            {
                foreach (var item in SeedField.text)
                {
                    seed += item;
                }
            }
        }
        else
        {
            seed = Random.Range(0, 65536);
        }
        SceneManager.LoadScene("Main");
    }
}
