using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsButton : MonoBehaviour
{
    public void OpenSettings()
    {
        Debug.Log("SettingsButton: Switching to Settings scene");
        SceneManager.LoadScene("02Settings");
    }

    public void GameScreen()
    {
        Debug.Log("SettingsButton: Switching to Gameplay scene");
        SceneManager.LoadScene("01Gameplay");
    }

    public void TitleScreen()
    {
        Debug.Log("SettingsButton: Switching to Title scene");
        SceneManager.LoadScene("00TitleScreen");
    }
}
