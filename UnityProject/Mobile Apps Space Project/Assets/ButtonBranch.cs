using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//I got the code from the tutorial: https://www.youtube.com/watch?v=Gt1PLvRf8rY
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

    }

    [System.Serializable]
    public class LinearSpawner
    {

    }
    [System.Serializable]
    public class CircularSpawner
    {

    }

    public GameObject[] buttonRefs;

    public enum ScaleMode { MatchWidthHeight, IndependentWidthHeight}
    public Vector2 referenceButtonSize;
    public Vector2 referenceScreenSize;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
