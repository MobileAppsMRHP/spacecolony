using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScreenOnZoom : MonoBehaviour {
    public GameObject activeCamera;
    public float zoomValueMax;
    public float zoomValueMin;
    float currentZoomValue;
	// Use this for initialization
	void Start () {
        currentZoomValue = activeCamera.GetComponent<Camera>().orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ChangeScreen()
    {
        if (currentZoomValue < zoomValueMax && currentZoomValue > zoomValueMin)
        {
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }
}
