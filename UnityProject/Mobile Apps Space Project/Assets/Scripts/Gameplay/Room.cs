using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    public struct RequiredResources
    {
        public float scraps;
        public float energy;
        public float money;

        public RequiredResources(float s, float e, float m)
        {
            scraps = s;
            energy = e;
            money = m;
        }
    }
    //public enum RoomType { Food, Bridge, Energy};
    public int peopleLimit;
    int roomLevel;
    public List<Crew> crewInThisRoom;
    public List<GameObject> crewLocations;
    public List<RequiredResources> UpgradeResources;
    public Shared.RoomTypes roomType;
    public bool currentlySelected;
    public GameManager gameManager;
    // Use this for initialization
    void Start () {
        UpgradeResources = new List<RequiredResources>()
        {
            new RequiredResources(1, 1, 1),
            new RequiredResources(2, 3, 1)
        };
        gameManager = GameManager.instance;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) //if there are touch events in the buffer to process...
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), new Vector2(0f, 0f)); //do a raycast to see what they hit
            if (hit.collider.name == gameObject.name) //if it hit something, anything, ....
            {
                Debug.Log("Room selected"); //log that something was hit by the touch event
                Debug.Log(hit.collider);
                currentlySelected = true;
            }
            else
            {
                Debug.Log("Room no longer selected");
                currentlySelected = false;
            }
        }
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
        {
            //Debug.Log("Too many people in this room");
            return false;
        }
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

    public void CrewIntoPosition(Crew crewToMove)
    {
        for (int i = 0; i < crewInThisRoom.Count; i++)
        {
            crewInThisRoom[i].transform.position = crewLocations[i].transform.position;
        }
    }

    void IncreaseResources()
    {
        switch (roomType)
        {
            case Shared.RoomTypes.food:
                //increase food
                break;
            case Shared.RoomTypes.energy:
                //increase
                break;
            default: //empty room
                //do nothing
            break;
        }
    }

    bool CrewMoving()
    {
        for (int i = 0; i < crewInThisRoom.Count; i++)
        {
            if(crewInThisRoom[i].GetComponent<DragAndDrop>().selected)
            {
                //Debug.Log("Crew moving");
                return true;
            }
        }
        return false;
    }
    public bool IncreaseLevel()
    {
        if (gameManager.resourceManager.GetResource(Shared.ResourceTypes.scraps) > UpgradeResources[roomLevel-1].scraps && 
            gameManager.resourceManager.GetResource(Shared.ResourceTypes.energy) > UpgradeResources[roomLevel - 1].energy && 
            gameManager.resourceManager.GetResource(Shared.ResourceTypes.money) > UpgradeResources[roomLevel - 1].money)
        {
            roomLevel++;
            return true;
        }
        return false;
    }
}
