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
            TextElement.color = new Color(0f, 255f, 0f);
            TextElement.text = "Logged in" + temp;
            Debug.Log("AuthButton: Found token in PlayerPrefs: " + temp);
        }
        else
        {
            TextElement.color = new Color(255f, 0f, 0f);
            TextElement.text = "Not logged in";
            Debug.Log("AuthButton: Did not find auth token in PlayerPrefs");
        }
    }

    public void LogOut()
    {
        GetComponent<AudioSource>().Play();
        TextElement.text = "Exit game to log out";
        TextElement.color = new Color(255f, 0f, 0f);
        Debug.Log("AuthButton: Erasing UID for auth");
        PlayerPrefs.DeleteKey(Shared.PlayerPrefs_AuthTokenKey);
        PlayerPrefs.Save();
    }
}
