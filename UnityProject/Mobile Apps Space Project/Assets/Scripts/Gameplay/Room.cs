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
    public bool selected;
    // Use this for initialization
    void Start () {
        UpgradeResources = new List<RequiredResources>()
        {
            new RequiredResources(1, 1, 1)

        };

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) //if there are touch events in the buffer to process...
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), new Vector2(0f, 0f)); //do a raycast to see what they hit
            if (hit.collider.tag == "Room") //if it hit something, anything, ....
            {
                Debug.Log("Room selected"); //log that something was hit by the touch event
                Debug.Log(hit.collider);
                gameObject.GetComponent<Animator>().enabled = !gameObject.GetComponent<Animator>().enabled; //toggle animator enabled status
                selected = true;
            }
            else
            {
                selected = false;
            }
        }
        if (crewInThisRoom.Count > 0 && !CrewMoving())
        {
            for (int i = 0; i < crewInThisRoom.Count; i++)
            {
                crewInThisRoom[i].transform.position = CrewIntoPosition(crewInThisRoom[i]);
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

    bool CrewMoving()
    {
        for (int i = 0; i < crewInThisRoom.Count; i++)
        {
            if(crewInThisRoom[i].GetComponent<DragAndDrop>().selected)
            {
                return true;
            }
        }
        return false;
    }
}
