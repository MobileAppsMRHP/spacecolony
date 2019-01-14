using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour {
    public GameObject[] pages = new GameObject[4];
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void swapPages(int current, int next)
    {
        pages[current].SetActive(false);
        pages[next].SetActive(true);
    }
}
