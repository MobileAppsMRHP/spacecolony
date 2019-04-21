using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour {
    public Button roomUpgradeButton;
    public GameManager gameManager;
    public GameObject roomHighlighter;
	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
		if (RoomUpgrade.FindSelectedRoom(gameManager) == null)
        {
            roomHighlighter.SetActive(false);
            roomUpgradeButton.interactable = false;
        }
        else
        {
            roomHighlighter.SetActive(true);
            roomHighlighter.transform.position = RoomUpgrade.FindSelectedRoom(gameManager).transform.position;
            roomUpgradeButton.interactable = true;
        }
	}
}
