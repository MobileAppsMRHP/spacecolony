using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtons : MonoBehaviour {

    public Text textShowed = null;

    public void changeWord(string word)
    {
        textShowed.text = word;
    }
}
