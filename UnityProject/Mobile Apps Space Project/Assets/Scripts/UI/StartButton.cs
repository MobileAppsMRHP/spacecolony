using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour {
    //protected bool loggedIn;
    //protected static UserAuthentication auth;
    // Use this for initialization
    void Start () {
        //auth = gameObject.AddComponent<UserAuthentication>();
        Debug.Log("DEBUG: Erasing playerprefs");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void ChangeScreen()
    {
        if (PlayerPrefs.HasKey(Shared.PlayerPrefs_AuthTokenKey))
        {
            Debug.Log("PlayerPrefs has a token, going to gameplay screen.");
            SceneManager.LoadScene("01Gameplay", LoadSceneMode.Single);
            /*Debug.Log("Unloading scene " + SceneManager.GetActiveScene().name);
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());*/
        }
        else 
        {
            Debug.Log("PlayerPrefs does NOT have a token, going to login screen.");
            SceneManager.LoadScene("Login", LoadSceneMode.Single);
            /*Debug.Log("Unloading scene " + SceneManager.GetActiveScene().name);
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());*/

        }
    }
}
