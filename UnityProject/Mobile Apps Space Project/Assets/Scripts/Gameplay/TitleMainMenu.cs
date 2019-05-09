using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMainMenu : MonoBehaviour {

    public void StartGame()
    { 
        Debug.Log("Loading scene 01Gameplay");
        SceneManager.LoadScene("01Gameplay", LoadSceneMode.Single);//SceneManager.GetActiveScene().buildIndex + 1);
        //GetComponent<AudioSource>().Play(); //plays too late, instead playing click on login scene load
        /*Debug.Log("Unloading scene 00TitleScreen");
        SceneManager.UnloadSceneAsync("00TitleScreen");*/
    }

    public void QuitGame()
    {
        GetComponent<AudioSource>().Play();
        Debug.Log("Quit!");
        Application.Quit();
    }
}
