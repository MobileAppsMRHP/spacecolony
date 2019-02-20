﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthButton : MonoBehaviour {
    public Text textShowed;
    public bool tracker;
    protected static UserAuthentication auth;

    void Awake(){
         textShowed.text="Logged Off";
         tracker = true;
         auth= new UserAuthentication();
}

    public void ChangeWordStatus()
    {
        if (tracker)
        {
            tracker = false;
        }
        else
        {
            auth.SigninWithEmailCredentialAsync();
            textShowed.text = "Logged in";
        }
    }
}