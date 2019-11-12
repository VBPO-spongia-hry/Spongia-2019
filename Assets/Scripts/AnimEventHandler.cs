using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventHandler : MonoBehaviour
{
    public void HideAnimEnd()
    {
        MainMenu mainMenu = Camera.main.GetComponent<MainMenu>();
        mainMenu.OnHideAnimEnd();
    }
}
