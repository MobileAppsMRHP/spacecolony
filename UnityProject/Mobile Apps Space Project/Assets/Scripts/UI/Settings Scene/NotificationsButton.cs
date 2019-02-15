using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsButton : MonoBehaviour {

    public Text TextElementToChange = null;
    public bool Enabled = true;

    public void Start()
    {
        UpdateButton();
    }

    public void ButtonPress()
    {
        UpdateButton();
    }

    private void UpdateButton()
    {
        if (Enabled)
        {
            //Actions to occur when button turns off go here. Will also run on start of script if Enabled=false.
            TextElementToChange.text = "Off";
            TextElementToChange.color = Color.red;
            Enabled = false;
        }
        else
        {
            //Actions to occur when button turns on go here. Will also run on start of script if Enabled=true.
            TextElementToChange.text = "On";
            TextElementToChange.color = Color.green;
            Enabled = true;
        }
    }

}
