﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthButton : MonoBehaviour {

    public Text textShowed = null;
    public bool tracker = true;
    protected static UserAuthentication auth = new UserAuthentication();

    public void ChangeWordStatus()
    {
        if (tracker)
        {
            textShowed.text = "Logged Off";
            tracker = false;
        }
        else
        {
            auth.SigninWithEmailCredentialAsync();
            textShowed.text = "Logged in";
        }
    }
}
