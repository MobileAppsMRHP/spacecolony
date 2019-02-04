using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeButton : MonoBehaviour {

    public Text textShowed = null;
    public bool tracker = true;

    public void ChangeWordNotification()
    {
        if (tracker)
        {
            textShowed.text = "Off";
            tracker = false;
        }
        else
        {
            textShowed.text = "On";
            tracker = true;
        }
    }

    public void ChangeWordStatus()
    {
        if (tracker)
        {
            textShowed.text = "Logged Off";
            tracker = false;
        }
        else
        {
            textShowed.text = "Logged On";
            tracker = true;
        }
    }
}
