﻿using System.Collections;
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
        if ((Input.GetMouseButtonDown(0) || Input.touchCount == 1) && inRoom)
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

            if (GetComponent<Crew>().DoCollisions)
            {
                GameManager.DebugLog("Running collision with " + collider.ToString() + " and " + GetComponent<Crew>().CrewName, DebugFlags.CollisionOps);
            }
            else
            {
                GameManager.DebugLog("SKIPPED Collision with " + collider.ToString() + " and " + GetComponent<Crew>().CrewName + " because crew wasn't ready.", DebugFlags.CollisionOps);
                return;
            }

            if (collider.tag == "Room")
            {
                
                //Debug.Assert(!droppedRoom.crewInThisRoom.Contains(droppedCrew));
                if (!droppedRoom.crewInThisRoom.Contains(droppedCrew) && droppedRoom.SpacesAvailable())
                { //if the room does not already contain this crew member && the room has spaces available
                    droppedRoom.AddPerson(droppedCrew); //add the crew member to the new room
                    if (oldRoom != null) //if the crew member had an old room...
                        oldRoom.RemovePerson(droppedCrew); //remove the crew member from the room it is currently in
                    GameManager.DebugLog("Moving " + droppedCrew.CrewName + " (" + droppedCrew.identifier + ") into room " + droppedRoom, DebugFlags.CollisionOps);
                    droppedCrew.AllData.RoomData.CurrentRoomStringForDB = droppedRoom.RoomUniqueIdentifierForDB;
                    droppedCrew.DatabaseUpdateRoomData();
                    droppedRoom.CrewIntoPosition();
                    droppedCrew.currentRoom = droppedRoom; //add the crew member to the room's list of the crew it contains
                }
                else if (!droppedRoom.crewInThisRoom.Contains(droppedCrew) && !droppedRoom.SpacesAvailable())
                {
                    Debug.Log("Too much people");
                    transform.position = initialPosition;
                }
                else if (droppedRoom.crewInThisRoom.Contains(droppedCrew))
                {
                    droppedRoom.CrewIntoPosition();
                    //GameManager.DebugLog("Can't enter room for some reason", DebugFlags.CollisionOps);
                }
                inRoom = true;

            }
            else if (collider.tag == "Background") //if it was dropped on a background object, don't move it anywhere.
            {
                transform.position = initialPosition;
                GameManager.DebugLog("Collided with background, has room; returning to original room", DebugFlags.CollisionOps);
            }
            else
            {
                transform.position = initialPosition;
                GameManager.DebugLog("It did none of the collision things?!? Collided with: '" + collider.tag + "' name: " + collider.transform.parent.gameObject.name, DebugFlags.CollisionOps);
            }
        }

    }
}
