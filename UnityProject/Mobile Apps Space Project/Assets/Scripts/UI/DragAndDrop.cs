using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour
{
    public GameObject mainCamera;
    public bool selected;
    Vector3 initialPosition;
    public Room currentRoom;
    bool inRoom;

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
            Debug.Log("touch ended");
            mainCamera.GetComponent<CamPan>().characterSelected = false;
        }
	}

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selected = true;
            mainCamera.GetComponent<CamPan>().characterSelected = true;
            initialPosition = transform.position;
            inRoom = false;
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (!collider.gameObject.GetComponent<Room>().crewInThisRoom.Contains(GetComponent<Crew>()) && collider.gameObject.GetComponent<Room>().SpacesAvailable())
        {
            collider.gameObject.GetComponent<Room>().AddPerson(gameObject.GetComponent<Crew>());
            currentRoom.RemovePerson(GetComponent<Crew>());
            currentRoom = collider.gameObject.GetComponent<Room>();
        }
        if (collider.tag == "Room") //
        {
            Debug.Log("Room dropped"); //
            transform.position = collider.GetComponent<Room>().CrewIntoPosition(GetComponent<Crew>());
            inRoom = true;
        }
        else if(collider.tag == "Background" && !inRoom)
        {
            transform.position = initialPosition;
        }

    }
}
