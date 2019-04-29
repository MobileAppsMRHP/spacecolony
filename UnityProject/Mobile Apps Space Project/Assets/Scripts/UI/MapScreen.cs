using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScreen : MonoBehaviour {
    public Room[] planets;
    public GameObject planetHighlighter;
    public Camera mainCamera;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (mainCamera.orthographicSize < 2)
        {
            for (int i=0; i < planets.Length; i++)
            {
                if (planets[i].currentlySelected)
                {
                    planetHighlighter.SetActive(true);
                    planetHighlighter.transform.position = planets[i].transform.position;
                    i = planets.Length + 1;
                }
            }
        }
        else
        {
            planetHighlighter.SetActive(false);
        }
	}
}
