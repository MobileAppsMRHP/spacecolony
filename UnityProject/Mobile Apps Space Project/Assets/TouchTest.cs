using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTest : MonoBehaviour {

    
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0)
        {
            gameObject.GetComponent<Animator>().enabled = !gameObject.GetComponent<Animator>().enabled; //toggle animator enabled status
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) //if there are touch events in the buffer to process...
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), new Vector2(0f, 0f)); //do a raycast to see what they hit
            if (hit.collider != null) //if it hit something, anything, ....
            {
                Debug.Log("Touched it"); //log that something was hit by the touch event
            }
        }
    }
}
