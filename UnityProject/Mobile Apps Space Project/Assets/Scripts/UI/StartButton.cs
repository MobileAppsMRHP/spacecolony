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
        
    }

    public void ChangeScreen()
    {
        if (PlayerPrefs.HasKey("UserAuthToken"))
        {
            Debug.Log("PlayerPrefs has a token, going to gameplay screen.");
            SceneManager.LoadScene("01Gameplay");
        }
        else 
        {
            Debug.Log("PlayerPrefs does NOT have a token, going to login screen.");
            SceneManager.LoadScene("login");

        }
    }
}
