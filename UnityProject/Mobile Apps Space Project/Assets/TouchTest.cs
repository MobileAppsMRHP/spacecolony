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
            gameObject.GetComponent<Animator>().enabled = !gameObject.GetComponent<Animator>().enabled;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), new Vector2(0f, 0f));
            if (hit.collider != null)
            {
                Debug.Log("Touched it");
            }
        }
    }
}
