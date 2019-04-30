using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTest : MonoBehaviour {

    
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) //if there are touch events in the buffer to process...
        {
            //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), new Vector2(0f, 0f)); //do a raycast to see what they hit
            //below built from https://answers.unity.com/questions/598492/how-do-you-set-an-order-for-2d-colliders-that-over.html
            RaycastHit2D hit = Physics2D.GetRayIntersection(new Ray(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), new Vector3(0, 0, 1))); //do a raycast to see what they hit
            if (hit.collider.tag == "Player") //if it hit something, anything, ....
            {
                //Debug.Log("Touched it"); //log that something was hit by the touch event
                //Debug.Log(hit.collider);
                gameObject.GetComponent<Animator>().enabled = !gameObject.GetComponent<Animator>().enabled; //toggle animator enabled status
            }
        }
    }

    public void Move()
    {
        gameObject.GetComponent<Animator>().enabled = !gameObject.GetComponent<Animator>().enabled;
    }
}
