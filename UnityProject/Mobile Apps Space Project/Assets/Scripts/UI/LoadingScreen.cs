using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {
    public GameManager gameManager;
    public GameObject mainScreen;

	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.IsDoneLoading)
        {
            mainScreen.SetActive(true);
            gameObject.SetActive(false);
        }
	}
}
