using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomUpgrade : MonoBehaviour {
    GameManager gameManager;
    public Room selectedRoom;
    public List<Text> resourceCosts;
    public Text selectedRoomName;
    public Text selectedRoomLevel;
    public Button upgradeButton;
    public GameObject upgradedInfo;
	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
    }
	
	// Update is called once per frame
	void Update () {
        selectedRoom = FindSelectedRoom(gameManager);
        if (selectedRoom == null)
        {
            for (int i = 0; i < resourceCosts.Count; i++)
            {
                resourceCosts[i].text = "";
            }
            upgradeButton.interactable = false;
            selectedRoomName.text = "Select a room";
            selectedRoomLevel.text = "";
        }
        else
        {
            upgradeButton.interactable = true;
            resourceCosts[0].text = "-" + ((int)selectedRoom.upgradeCosts.x + 1).ToString();
            resourceCosts[1].text = "-" + ((int)selectedRoom.upgradeCosts.y + 1).ToString();
            resourceCosts[2].text = "-" + ((int)selectedRoom.upgradeCosts.z + 1).ToString();
            selectedRoomName.text = selectedRoom.RoomType.ToString();
            selectedRoomLevel.text = "L: " + selectedRoom.roomLevel;
            upgradeButton.interactable = selectedRoom.CanIncreaseLevel();
        }
    }

    public static Room FindSelectedRoom(GameManager gameManager)
    {
        for (int i=0; i<gameManager.Rooms.Count; i++)
        {
            if (gameManager.Rooms[i].currentlySelected)
            {
                return gameManager.Rooms[i];
            }
        }
        return null;
    }

    public void UpgradeRoom()
    {
        selectedRoom.IncreaseLevel();
        upgradedInfo.SetActive(true);
    }
}
