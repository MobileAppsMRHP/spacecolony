using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    enum RoomType { Food, Bridge};
    int peopleLimit;
    int roomLevel;
    List<Crew> crewInThisRoom;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ChangeCrew(Crew member)
    {
        //make a check to see if that crew is in that list.
        crewInThisRoom.Remove(member);
    }
}
