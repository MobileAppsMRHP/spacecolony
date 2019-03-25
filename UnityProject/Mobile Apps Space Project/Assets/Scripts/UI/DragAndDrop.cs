using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* 
    Handles collisions of crew members with rooms and the backdrop elements, moving them from room to room
     */

public class DragAndDrop : MonoBehaviour
{
    public bool selected;
    Vector3 initialPosition;
    //public Room currentRoom;
    public bool inRoom;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (selected)
        {
            Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(cursorPos.x, cursorPos.y);
        }
        if (selected && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            selected = false;
            //Debug.Log("touch ended");
        }
	}

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selected = true;
            initialPosition = transform.position;
            inRoom = false;
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (!selected)
        {
            Room oldRoom = GetComponent<Crew>().currentRoom;
            Room droppedRoom = collider.gameObject.GetComponent<Room>();
            Crew droppedCrew = GetComponent<Crew>();

            //Debug.Log("Running collision with " + collider.ToString() + " and " + GetComponent<Crew>().name);

            if (collider.tag == "Room")
            {
                if (!droppedRoom.crewInThisRoom.Contains(droppedCrew) && droppedRoom.SpacesAvailable())
                { //if the room does not already contain this crew member && the room has spaces avalible
                    droppedRoom.AddPerson(droppedCrew); //add the crew member to the new room
                    if (oldRoom != null) //if the crew member had an old room...
                        oldRoom.RemovePerson(droppedCrew); //remove the crew member from the room it is currently in
                    Debug.Log("Moving " + droppedCrew.crewName + " into room " + droppedRoom);
                }
                else if (!droppedRoom.crewInThisRoom.Contains(droppedCrew))
                {
                    //Debug.Log("Can't enter room for some reason");
                }
                droppedRoom.CrewIntoPosition(droppedCrew);
                inRoom = true;
                droppedCrew.currentRoom = droppedRoom; //add the crew memeber to the room's list of the crew it contains
            }
            else if (collider.tag == "Background" && inRoom) //if it was dropped on a background object, don't move it anywhere.
            {
                transform.position = initialPosition;
                Debug.Log("returning to original room");
            }
        }

    }
}
