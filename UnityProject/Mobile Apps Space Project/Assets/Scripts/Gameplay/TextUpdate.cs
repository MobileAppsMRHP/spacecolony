using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUpdate : MonoBehaviour {
    public GameManager gameManager;
    public Text displayText;
	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance; //drag-and-drop set didnt seem to work?
        displayText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(gameManager);
        //displayText.text = "" + gameManager.GetResource("food");
	}
}
