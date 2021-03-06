﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsButton : MonoBehaviour
{
    public void OpenSettings()
    {
        PlaySound();
        Debug.Log("SettingsButton: Loading settings scene");
        SceneManager.LoadScene("02Settings", LoadSceneMode.Additive);
        GameManager.instance.ChangeCurrentScene(3);
        /*Debug.Log("Unloading scene " + SceneManager.GetActiveScene().name);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());*/
    }

    public void QuitGame()
    {
        PlaySound();
        Application.Quit();
    }

    public void CloseMap()
    {
        PlaySound();
        Debug.Log("SettingsButton: Unloading map scene");
        //SceneManager.LoadScene("01Gameplay", LoadSceneMode.Single);
        SceneManager.UnloadSceneAsync("03Map");
        GameManager.instance.ChangeCurrentScene(2);
        /*Debug.Log("Unloading scene " + SceneManager.GetActiveScene().name);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());*/
    }

    public void CloseSettings()
    {
        PlaySound();
        Debug.Log("SettingsButton: Unloading settings screen");
        //SceneManager.LoadScene("00TitleScreen", LoadSceneMode.Single);
        SceneManager.UnloadSceneAsync("02Settings");
        GameManager.instance.ChangeCurrentScene(2);
        /*Debug.Log("Unloading scene " + SceneManager.GetActiveScene().name);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());*/
    }

    public void PlaySound()
    {
        var gm = GameManager.instance;
        if (gm == null)
            GetComponent<AudioSource>().Play();
        else
            gm.PlayButtonSound();
    }
}
