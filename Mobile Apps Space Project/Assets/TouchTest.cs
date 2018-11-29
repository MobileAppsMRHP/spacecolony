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
	}
}
