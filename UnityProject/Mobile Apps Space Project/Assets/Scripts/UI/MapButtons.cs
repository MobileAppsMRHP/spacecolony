using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapButtons : MonoBehaviour {

    public Camera Cam;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public void Zoom () {
        Cam.fieldOfView = 60;
	}
}
