using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManage : MonoBehaviour {
    public GameManager gameManager;
    public int sceneNum;
    public GameObject sceneUI;
	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
		if ((GameManager.SceneSelected)sceneNum == gameManager.sceneCurrentlySelected)
        {
            sceneUI.SetActive(true);
        }
        else
        {
            sceneUI.SetActive(false);
        }
	}
}
