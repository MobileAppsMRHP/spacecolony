using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour {
    public Button roomUpgradeButton;
    public GameManager gameManager;
	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
		if (RoomUpgrade.FindSelectedRoom(gameManager) == null)
        {
            roomUpgradeButton.interactable = false;
        }
        else
        {
            roomUpgradeButton.interactable = true;
        }
	}
}
