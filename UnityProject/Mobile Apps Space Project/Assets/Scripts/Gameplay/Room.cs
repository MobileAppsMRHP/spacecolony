using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    public struct RequiredResources
    {
        float scraps;
        float energy;
        float money;

        public RequiredResources(float s, float e, float m)
        {
            scraps = s;
            energy = e;
            money = m;
        }
    }
    public enum RoomType { Food, Bridge, Energy};
    public int peopleLimit;
    int roomLevel;
    public List<Crew> crewInThisRoom;
    public List<GameObject> crewLocations;
    public List<RequiredResources> UpgradeResources;
    public RoomType roomType;
    // Use this for initialization
    void Start () {
        UpgradeResources = new List<RequiredResources>()
        {
            new RequiredResources(1, 1, 1)

        };

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void ChangeCrew(Crew member)
    {
        //make a check to see if that crew is in that list.
        crewInThisRoom.Remove(member);
    }

    public bool SpacesAvailable()
    {
        if (crewInThisRoom.Count < peopleLimit)
            return true;
        else
            return false;
    }

    public float SpacesLeft()
    {
        return peopleLimit - crewInThisRoom.Count;
    }

    public void AddPerson(Crew newCrew)
    {
        crewInThisRoom.Add(newCrew);
    }

    public void RemovePerson(Crew crewToRemove)
    {
        crewInThisRoom.Remove(crewToRemove);
    }

    public Vector3 CrewIntoPosition(Crew crewToMove)
    {
        int index = crewInThisRoom.IndexOf(crewToMove);
        return crewLocations[index].transform.position;
    }

    void IncreaseResources()
    {
        switch (roomType)
        {
            case RoomType.Food:
                //increase food
                break;
            case RoomType.Energy:
                //increase
                break;
        }
    }
}
