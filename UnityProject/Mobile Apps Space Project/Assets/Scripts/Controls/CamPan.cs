using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPan : MonoBehaviour {
    public bool characterSelected;
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

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && !zooming && !characterSelected)
        {
            Vector3 changeDirection = touchInitialLocation - Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position));
            Camera.main.transform.position += changeDirection;
            //Camera.main.transform.position = Vector3.Lerp(touchInitialLocation, Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), .01f);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && !zooming && characterSelected)
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position));
            //Debug.Log(cameraPosition);
            //Debug.Log(touchPosition);
            Debug.Log(Vector3.Distance(cameraPosition, touchPosition));
            if (Vector3.Distance(cameraPosition, touchPosition) > 0.6)
            {
                Camera.main.transform.position = Vector3.Lerp(cameraPosition, touchPosition, .06f);
            }
        }

    }
}
