using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour {

    public Room prefab;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public void CreateRoom()
    {
        Room newRoom = Instantiate(prefab, this.transform); //create new room prefab at the spawner
        GameManager.instance.Rooms.Add(newRoom);
    }
}
