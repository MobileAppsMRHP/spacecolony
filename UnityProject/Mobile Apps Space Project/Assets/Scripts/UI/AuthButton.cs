using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AuthButton : MonoBehaviour {
    public Text textShowed;
    public bool tracker;
    protected static UserAuthentication auth;
    public string message;

    void Awake() { 
         tracker = false;
         auth= gameObject.AddComponent<UserAuthentication>();
         message = "Logged Off";
         textShowed.text = message;
    }

    public void ChangeWordStatus()
    {
        if (tracker)
        {
            tracker = false;
            textShowed.text = message;
        }
        else
        {
            auth.EnableUI();
            SceneManager.LoadScene("Login");
            textShowed.text = "Logged in";
            tracker = true;
        }
    }
}
