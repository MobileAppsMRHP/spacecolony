using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//I got the code from the tutorial: https://www.youtube.com/watch?v=Gt1PLvRf8rY and https://www.youtube.com/watch?v=T4s312Evmbk&list=PL4CCSwmU04MhI6oeuUZ-NRvhEye65PlFx&index=5
public class ButtonBranch : MonoBehaviour {
    public class ButtonScale
    {
        enum ScaleMode { MatchWidthHeight, IndependentWidthHeight}
        ScaleMode mode;
        Vector2 referenceButtonSize;

        [HideInInspector]
        public Vector2 referenceScreenSize;
        public Vector2 newButtonSize;

        public void Initialize(Vector2 refButtonSize, Vector2 refScreenSize, int scaleMode)
        {
            mode = (ScaleMode)scaleMode;
            referenceButtonSize = refButtonSize;
            referenceScreenSize = refScreenSize;
            SetNewButtonSize();
        }

        void SetNewButtonSize()
        {
            newButtonSize.x = (referenceButtonSize.x * Screen.width) / referenceScreenSize.x; //ratio
            if (mode == ScaleMode.MatchWidthHeight)
            {
                newButtonSize.y = newButtonSize.x;
            }
            else if (mode == ScaleMode.IndependentWidthHeight)
            {
                newButtonSize.y = (referenceButtonSize.y * Screen.height) / referenceScreenSize.y; //ratio
            }
        }
    }

    [System.Serializable]
    public class RevealSettings
    {
        public enum RevealOption { Linear, Circular};
        public RevealOption option;
        public float moveSpeed = 0.01f;
        public float fadeSpeed = 5f;
        public bool revealOnStart = false;
        [HideInInspector]
        public bool opening = false;
        [HideInInspector]
        public bool ending = false;
    }

    [System.Serializable]
    public class LinearSpawner
    {
        public enum RevealStyle { SlideToPosition, FadeInAtPosition};
        public RevealStyle revealStyle;
        public Vector2 direction = new Vector2(0, 1); //slide down
        public float baseSpacing = 5f;
        public int NumOffset = 0; //How many button spaces offset, needed sometimes when there are multiple branches
        [HideInInspector]
        public float spacing = 5f;

        public void FitSpacingToScreenSize(Vector2 refScreenSize) //adjusts spacing for different screen sizes
        {
            float refScreenFloat = (refScreenSize.x + refScreenSize.y) / 2;
            float screenFloat = (Screen.width + Screen.height) / 2;
            spacing = (baseSpacing * screenFloat) / refScreenFloat;
        }
    }
    [System.Serializable]
    public class CircularSpawner
    {
        public enum RevealStyle { SlideToPosition, FadeInAtPosition };
        public RevealStyle revealStyle;
        public Angle angle;
        public float baseDistFromBrancher = 20f;
        [HideInInspector]
        public float distFromBrancher = 0f;

        [System.Serializable]
        public struct Angle { public float minAngle; public float maxAngle; } //struct is a value type that can hold a group of variable and even methods

        public void FitDistanceToScreenSize(Vector2 refScreenSize) //adjusts distances for different screen sizes
        {
            float refScreenFloat = (refScreenSize.x + refScreenSize.y) / 2;
            float screenFloat = (Screen.width + Screen.height) / 2;
            distFromBrancher = (baseDistFromBrancher * screenFloat) / refScreenFloat;
        }
    }

    public GameObject[] buttonRefs; //prefabs
    [HideInInspector]
    public List<GameObject> buttons;

    public enum ScaleMode { MatchWidthHeight, IndependentWidthHeight}
    public ScaleMode mode;
    public Vector2 referenceButtonSize;
    public Vector2 referenceScreenSize;

    ButtonScale buttonScale = new ButtonScale();
    public RevealSettings revealSettings = new RevealSettings();
    public LinearSpawner linSpawner = new LinearSpawner();
    public CircularSpawner circSpawner = new CircularSpawner();

    float lastScreenWidth = 0f;
    float lastScreenHeight = 0f;

    // Use this for initialization
    void Start () {
        buttons = new List<GameObject>();
        buttonScale = new ButtonScale();
        lastScreenHeight = Screen.height;
        lastScreenWidth = Screen.width;
        buttonScale.Initialize(referenceButtonSize, referenceScreenSize, (int)mode);
        circSpawner.FitDistanceToScreenSize(buttonScale.referenceScreenSize);
        linSpawner.FitSpacingToScreenSize(buttonScale.referenceScreenSize);
        SpawnButtons();
	}
	
	// Update is called once per frame
	void Update () {
		if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenHeight = Screen.height;
            lastScreenWidth = Screen.width;
            buttonScale.Initialize(referenceButtonSize, referenceScreenSize, (int)mode);
            circSpawner.FitDistanceToScreenSize(buttonScale.referenceScreenSize);
            linSpawner.FitSpacingToScreenSize(buttonScale.referenceScreenSize);
            SpawnButtons();
        }

        if (revealSettings.opening)
        {
            //if (!revealSettings.spawned)
                SpawnButtons();

            switch (revealSettings.option) //like a if statement. This switch statement determines the reveal style.
            {
                case RevealSettings.RevealOption.Linear:
                    switch (linSpawner.revealStyle)
                    {
                        case LinearSpawner.RevealStyle.SlideToPosition: RevealLinearlyNormal(); break;
                        case LinearSpawner.RevealStyle.FadeInAtPosition: RevealLinearlyFade(); break;
                    }

                    break;
                    
                case RevealSettings.RevealOption.Circular:
                    switch (circSpawner.revealStyle)
                    {
                        case CircularSpawner.RevealStyle.SlideToPosition: RevealCircularNormal(); break;
                        case CircularSpawner.RevealStyle.FadeInAtPosition: RevealCircularFade(); break;
                    }

                    break;
            }
        }
	}

    public void SpawnButtons()
    {
        revealSettings.opening = true;
        //clear button list
        for (int i = buttons.Count; i >= 0; i--)
            Destroy(buttons[i]);
        buttons.Clear();

        ClearCommonButtonBranchers(); //clears any other button brancher that has the same parent

        for (int i=0; i<buttonRefs.Length; i++)
        {
            GameObject b = Instantiate(buttonRefs[i] as GameObject);
            b.transform.SetParent(transform); // make button child of button branch
            b.transform.position = transform.position;
            if (linSpawner.revealStyle == LinearSpawner.RevealStyle.FadeInAtPosition || circSpawner.revealStyle == CircularSpawner.RevealStyle.FadeInAtPosition)
            {
                Color c = b.GetComponent<Image>().color;
                c.a = 0; //Set button color alpha value to 0
                b.GetComponent<Image>().color = c;

                if (b.GetComponentInChildren<Text>()) //Set button text color alpha value to 0
                {
                    c = b.GetComponentInChildren<Text>().color;
                    c.a = 0;
                    b.GetComponentInChildren<Text>().color = c;
                }
            }
        }
    }

    void RevealLinearlyNormal()
    {
        for (int i=0; i<buttonRefs.Length; i++)
        {
            Vector3 targetPos; //position for button to move toward
            RectTransform buttonRect = buttons[i].GetComponent<RectTransform>();
            //set size
            buttonRect.sizeDelta = new Vector2(buttonScale.newButtonSize.x, buttonScale.newButtonSize.y); //width of the button
            //set pos
            targetPos.x = linSpawner.direction.x * ((i + linSpawner.NumOffset) * (buttonRect.sizeDelta.x + linSpawner.baseSpacing)) + transform.position.x;
            targetPos.y = linSpawner.direction.y * ((i + linSpawner.NumOffset) * (buttonRect.sizeDelta.y + linSpawner.baseSpacing)) + transform.position.y;
            targetPos.z = 0;

            buttonRect.position = Vector3.Lerp(buttonRect.position, targetPos, revealSettings.moveSpeed * Time.deltaTime);
        }
    }

    void RevealLinearlyFade()
    {
        for (int i = 0; i < buttonRefs.Length; i++)
        {
            Vector3 targetPos; //position for button to move toward
            RectTransform buttonRect = buttons[i].GetComponent<RectTransform>();
            //set size
            buttonRect.sizeDelta = new Vector2(buttonScale.newButtonSize.x, buttonScale.newButtonSize.y); //width of the button
            //set pos
            targetPos.x = linSpawner.direction.x * ((i + linSpawner.NumOffset) * (buttonRect.sizeDelta.x + linSpawner.baseSpacing)) + transform.position.x;
            targetPos.y = linSpawner.direction.y * ((i + linSpawner.NumOffset) * (buttonRect.sizeDelta.y + linSpawner.baseSpacing)) + transform.position.y;
            targetPos.z = 0;

            ButtonFader previousButtonFader;
            if (i > 0)
                previousButtonFader = buttons[i - 1].GetComponent<ButtonFader>(); //To prevent the next button from fading if the previous button hasn't finieshed fading yet
            else
                previousButtonFader = null; //First button in the list
            ButtonFader buttonFader = buttons[i].GetComponent<ButtonFader>();

            if (previousButtonFader)
            {
                if (previousButtonFader.faded) //When the previous button is done fading
                {
                    buttons[i].transform.position = targetPos;
                    if (buttonFader)
                    {
                        buttonFader.Fade(revealSettings.fadeSpeed);
                    }
                    else
                        Debug.LogError("You forgot to set a ButtonFader to button number " + i);
                }
            }
            else
            {
                buttons[i].transform.position = targetPos;
                if (buttonFader)
                {
                    buttonFader.Fade(revealSettings.fadeSpeed);
                }
                else
                    Debug.LogError("You forgot to set a ButtonFader to button number " + i);
            }
        }
    }

    void RevealCircularNormal()
    {

    }

    void RevealCircularFade()
    {

    }

    void ClearCommonButtonBranchers()
    {
        GameObject[] branchers = GameObject.FindGameObjectsWithTag("ButtonBrancher");
        foreach (GameObject brancher in branchers)
        {
            if (brancher.transform.parent == transform.parent) //checks to see if it is the same parent
            {
                ButtonBranch bb = brancher.GetComponent<ButtonBranch>();
                for (int i = bb.buttons.Count; i >= 0; i--)
                    Destroy(bb.buttons[i]);
                bb.buttons.Clear();
            }
        }
    }
}
