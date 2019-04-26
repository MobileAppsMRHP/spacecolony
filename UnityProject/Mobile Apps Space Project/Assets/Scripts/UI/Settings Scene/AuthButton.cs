using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthButton : MonoBehaviour
{
    public Text TextElement;
    //public bool tracker;
    //protected static UserAuthentication auth;
    //public string message;

    void Awake()
    {
        /*tracker = false;
        //auth= gameObject.AddComponent<UserAuthentication>();
        message = "Logged Off";
        TextElement.text = message;*/
        UpdateText();
    }

    public void UpdateText()
    {
        if (PlayerPrefs.HasKey(Shared.PlayerPrefs_AuthTokenKey))
        {
            string temp = PlayerPrefs.GetString(Shared.PlayerPrefs_AuthTokenKey);
            TextElement.text = "Logged in:\n" + temp;
            Debug.Log("AuthButton: Found token in PlayerPrefs: " + temp);
        }
        else
        {
            TextElement.text = "Not logged in";
            Debug.Log("AuthButton: Did not find auth token in PlayerPrefs");
        }
    }
}
