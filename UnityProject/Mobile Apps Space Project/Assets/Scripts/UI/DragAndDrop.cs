using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour
{ 
    private bool selected;

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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(0f, 0f), 0, 0); //do a raycast to see what they hit
            Debug.Log(hit.collider.tag);
            if (hit.collider.tag == "Room") //if it hit something, anything, ....
            {
                Debug.Log("Room dropped"); //log that something was hit by the touch event
                Debug.Log(hit.collider);
                transform.position = hit.collider.transform.position;
            }
        }
	}

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
            selected = true;
    }

    private void OnTriggerStay2D(Collider collider)
    {
        Debug.Log(collider.tag);
        if (collider.tag == "Room") //if it hit something, anything, ....
        {
            Debug.Log("Room dropped"); //log that something was hit by the touch event
            transform.position = collider.transform.position;
        }
    }
}
