using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomUpgrade : MonoBehaviour {
    GameManager gameManager;
    public Room selectedRoom;
    public List<Text> resourceCosts;
	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
    }
	
	// Update is called once per frame
	void Update () {
        selectedRoom = FindSelectedRoom();
        resourceCosts[0].text = "-" + selectedRoom.upgradeCosts.x.ToString();
        resourceCosts[1].text = "-" + selectedRoom.upgradeCosts.y.ToString();
        resourceCosts[2].text = "-" + selectedRoom.upgradeCosts.z.ToString();

    }

    Room FindSelectedRoom()
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

    void UpgradeRoom(Room selectRoom)
    {
        /*if (selectRoom.IncreaseLevel())
        {
            spawn image or change text;
        }  
        */
    }
}
