using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUpgrade : MonoBehaviour {
    GameManager gameManager;
    public Room selectedRoom;
	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
    }
	
	// Update is called once per frame
	void Update () {
        selectedRoom = FindSelectedRoom();
	}

    Room FindSelectedRoom()
    {
        for (int i=0; i<gameManager.Rooms.Count; i++)
        {
            if (gameManager.Rooms[i].selected)
            {
                return gameManager.Rooms[i];
            }
        }
        return null;
    }
}
