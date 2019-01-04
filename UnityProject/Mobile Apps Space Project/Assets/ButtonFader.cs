using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A lot of the code is from https://www.youtube.com/watch?v=_kDWjDaEZhI&list=PL4CCSwmU04MhI6oeuUZ-NRvhEye65PlFx&index=7
public class ButtonFader : MonoBehaviour {
    public bool faded = false;

    Image buttonImage;
    Text txt;
    Color buttonColor;
    Color textColor;
    bool startFade = false;
    float speed = 0;
    bool initialized = false;

	// Use this for initialization
	void Start () {
        Initialize();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (startFade)
        {
            Fade(speed);
            if (buttonColor.a > 0)
                faded = true;
        }
	}

    void Initialize()
    {
        startFade = false;
        faded = false;
        buttonImage = GetComponent<Image>();
        buttonColor = buttonImage.color;

        if (GetComponentInChildren<Text>())
        {
            txt = GetComponentInChildren<Text>();
            textColor = txt.color;
        }
        initialized = true;
    }

    public void Fade(float rate)
    {
        if (!initialized)
            Initialize();

        speed = rate;
        startFade = true;

        buttonColor.a += rate;
        buttonImage.color = buttonColor;
        if (txt)
        {
            textColor.a += rate;
            txt.color = textColor;
        }
    }
}
