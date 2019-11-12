using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button HowToPlayButton;
    public Button BackButton;
    public Animation Animation;

    public void ShowHowToPlay()
    {
        Animation.Play("Show");
    }
    public void HideHowToPlay()
    {
        Animation.Play("Hide");
    }
}
