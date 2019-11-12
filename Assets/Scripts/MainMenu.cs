using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public Animation Animation;
    public Animation SettingsAnim;
    public Animation MultiplayerAnim;
    public Animation HowToPlayAnim;
    public Dropdown dropdown;
    Vector2Int[] resolutions; 
    Vector2Int resolution;
    public TextMeshProUGUI MainText;
    public AudioMixer audioMixer;
    public Slider MusicSlider;
    public Slider SFXSlider;
    public Text MessageText;
    public InputField inputField;
    public Toggle fullscreen;

    private string _message;
    public string message {
        get { return _message; }
        set {
            _message = value;
            MessageText.text = _message;
            MessageText.GetComponent<Animation>().Play();
        }
    }

    enum action { Howtoplay, singleplayer, settings, quit, none };
    private action Action = action.settings;

    // Start is called before the first frame update
    void Start()
    {
        SaveData saveData = GameSaver.LoadGame();
        if (saveData != null)
        {
            LoadData(saveData);
        }
        Vector2Int[] resolutions = { new Vector2Int(3840, 2160), new Vector2Int(2560, 1440), new Vector2Int(1920, 1080), new Vector2Int(1280, 720), new Vector2Int(854,480)};
        resolution = new Vector2Int(Display.main.systemWidth, Display.main.systemHeight);
        Animation.Play("Show");
        int height = Display.main.systemHeight;
        int width = Display.main.systemWidth;
        foreach (Vector2 res in resolutions)
        {
            if (res.x <= width && res.y <= height)
            {
                dropdown.options.Add(new Dropdown.OptionData(res.x + "×" + res.y));
            }
        }
    }

    public void changeVolume()
    {
        audioMixer.SetFloat("SFX", SFXSlider.value);
        audioMixer.SetFloat("Music", MusicSlider.value);
    }

    public void SinglePlayer()
    {
        Action = action.singleplayer;
        Animation.Play("Hide");
    }

    public void HowToplay()
    {
        Action = action.Howtoplay;
        Animation.Play("Hide");
    }



    public void Exit()
    {
        Action = action.quit;
        Animation.Play("Hide");
    }

    public void Settings()
    {
        Action = action.settings;
        Animation.Play("Hide");
        MainText.SetText("Settings");
    }
    
    public void BackSettings()
    {
        SettingsAnim.Play("Hide");
        Animation.Play("Show");
        Screen.SetResolution(resolution.x, resolution.y, fullscreen.isOn);
        MainText.SetText("Naval Transport Simulator");
        changeVolume();
        SaveData saveData = GetSaveData();
        GameSaver.Savegame(saveData);
    }
    
    public void BackHowToPlay()
    {
        HowToPlayAnim.Play("Hide");
        Animation.Play("Show");
        MainText.SetText("Naval Transport Simulator");
        Action = action.none;
    }

    public void OnHideAnimEnd()
    {
        switch (Action)
        {
            case action.singleplayer:
                MultiplayerAnim.Play("Show");
                MainText.SetText("Create Game");
                break;
            case action.Howtoplay:
                HowToPlayAnim.Play("Show");
                MainText.SetText("How To Play");
                break;
            case action.settings:
                SettingsAnim.Play("Show");
                break;
            case action.quit:
                Application.Quit();
                break;
            default:
                break;
        }
        Action = action.none;
    }

    public void dropDownChanged()
    {
        Dropdown.OptionData optionData = dropdown.options[dropdown.value];
        string options = optionData.text;
        string[] option = options.Split('×');
        resolution.x = int.Parse(option[0]);
        resolution.y = int.Parse(option[1]);
    }

    public SaveData GetSaveData()
    {
        SaveData saveData = new SaveData();
        saveData.MusicVol = MusicSlider.value;
        saveData.SFXVol = SFXSlider.value;
        saveData.resX = resolution.x;
        saveData.resY = resolution.y;
        saveData.fullscreen = fullscreen.isOn;
        if(inputField.text != string.Empty)
        {
            saveData.name = inputField.text;
        }
        return saveData;
    }

    public void LoadData(SaveData data)
    {
        SFXSlider.value = data.SFXVol;
        MusicSlider.value = data.MusicVol;
        changeVolume();
        Screen.SetResolution(data.resX, data.resY, data.fullscreen);
        dropdown.value = dropdown.options.IndexOf(new Dropdown.OptionData(data.resX + "×" + data.resY));
        inputField.text = data.name;
    }
}
