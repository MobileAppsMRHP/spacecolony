using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPan : MonoBehaviour {
    public GameObject topLeftBorder;
    public GameObject bottomRightBorder;
    public GameObject mainScreen;
    bool characterSelected;
    bool zooming;
    Vector3 touchInitialLocation;
    float minXPos;
    float minYPos;
    float maxXPos;
    float maxYPos;
	// Use this for initialization
	void Start () {
        minXPos = topLeftBorder.transform.position.x;
        minYPos = bottomRightBorder.transform.position.y;
        maxXPos = bottomRightBorder.transform.position.x;
        maxYPos = topLeftBorder.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        zooming = GetComponent<CamZoom>().isZooming;
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchInitialLocation = Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position));
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && !zooming && !characterSelected && mainScreen.activeInHierarchy)
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
            //Debug.Log(Vector3.Distance(cameraPosition, touchPosition));
            if (Vector3.Distance(cameraPosition, touchPosition) > 0.6)
            {
                Camera.main.transform.position = Vector3.Lerp(cameraPosition, touchPosition, .06f);
            }
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) //if there are touch events in the buffer to process...
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), new Vector2(0f, 0f)); //do a raycast to see what they hit
            if (hit.collider.tag == "Player") //if it hit something, anything, ....
            {
                Debug.Log("Touched it"); //log that something was hit by the touch event
                Debug.Log(hit.collider);
                characterSelected = true;
            }
            else
            {
                characterSelected = false;
            }
        }
        if (transform.position.x > maxXPos)
        {
            transform.position = new Vector3(maxXPos, transform.position.y, transform.position.z);
        }
        if (transform.position.x < minXPos)
        {
            transform.position = new Vector3(minXPos, transform.position.y, transform.position.z);
        }
        if (transform.position.y > maxYPos)
        {
            transform.position = new Vector3(transform.position.x, maxYPos, transform.position.z);
        }
        if (transform.position.y < minYPos)
        {
            transform.position = new Vector3(transform.position.x, minYPos, transform.position.z);
        }
    }
}
