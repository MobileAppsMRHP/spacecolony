using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null; //singleton pattern
	// Use this for initialization
	void Awake () { //Only initializes once
		if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
