using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapPlanets : MonoBehaviour {
    public bool currentlySelected;
    public float resourceNum;
    
	// Use this for initialization
	void Start () {
        currentlySelected = false;
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) //if there are touch events in the buffer to process...
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), new Vector2(0f, 0f)); //do a raycast to see what they hit
                                                                                                                                     //Debug.Log(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId));
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                //do nothing
            }
            else if (hit.collider == null)
            {
                //do nothing
            }
            else if (hit.collider.name == gameObject.name)
            {
                Debug.Log(hit.collider);
                currentlySelected = true;
            }
            else
            {
                Debug.Log("nothing hit on map screen");
                currentlySelected = false;
            }
        }
    }
}
