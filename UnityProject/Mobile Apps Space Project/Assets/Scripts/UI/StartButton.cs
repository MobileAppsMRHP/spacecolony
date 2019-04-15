using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour {
    protected bool loggedIn;
    protected static UserAuthentication auth;
    // Use this for initialization
    void Start () {
        auth = gameObject.AddComponent<UserAuthentication>();
        loggedIn = auth.UserLoggedIn();
    }

    public void ChangeScreen()
    {
        if (loggedIn == true)
        {
            SceneManager.LoadScene("01Gameplay");
        }
        else 
        {
            SceneManager.LoadScene("login");

        }
    }
}
