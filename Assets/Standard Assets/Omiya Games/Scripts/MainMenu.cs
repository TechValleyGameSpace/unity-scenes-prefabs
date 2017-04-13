using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    bool isClicked = false;

    public void OnLevelClicked(string sceneName)
    {
        if (isClicked == false)
        {
            Application.LoadLevel(sceneName);
            isClicked = true;
        }
    }

    public void OnQuitClicked()
    {
        if (isClicked == false)
        {
            isClicked = true;
            Application.Quit();
        }
    }
}
