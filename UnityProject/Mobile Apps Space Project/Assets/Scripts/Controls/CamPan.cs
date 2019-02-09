using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPan : MonoBehaviour {
    public bool stopPan;
    bool zooming;
    Vector3 touchInitialLocation;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        zooming = GetComponent<CamZoom>().isZooming;
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchInitialLocation = Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position));
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && !zooming && !stopPan)
        {
            Vector3 changeDirection = touchInitialLocation - Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position));
            Camera.main.transform.position += changeDirection;
        }

    }
}
