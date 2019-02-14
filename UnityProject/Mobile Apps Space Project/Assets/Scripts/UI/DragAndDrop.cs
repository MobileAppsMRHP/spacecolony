using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour
{
    public GameObject mainCamera;
    private bool selected;
    Vector3 initialPosition;

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
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        
        if (collider.tag == "Room" && collider.gameObject.GetComponent<Room>().SpacesAvailable()) //
        {
            Debug.Log("Room dropped"); //
            transform.position = collider.transform.position;
            collider.gameObject.GetComponent<Room>().AddPerson(gameObject.GetComponent<Crew>());
            GetComponent<Crew>().GetCurrentRoom().RemovePerson(GetComponent<Crew>());
            GetComponent<Crew>().ChangeCurrentRoom(collider.gameObject.GetComponent<Room>());
        }
        else
        {
            transform.position = initialPosition;
        }

    }
}
