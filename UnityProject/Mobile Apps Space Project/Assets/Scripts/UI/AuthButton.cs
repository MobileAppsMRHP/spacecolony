using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthButton : MonoBehaviour {
    public Text textShowed;
    public bool tracker;
    protected static UserAuthentication auth;
    public string message;

    void Awake() { 
         tracker = true;
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
            auth.SigninWithEmailCredentialAsync();
            textShowed.text = "Logged in";
        }
    }
}
