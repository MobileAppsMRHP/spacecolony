using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapScreen : MonoBehaviour {
    public MapPlanets[] planets;
    public GameObject planetHighlighter;
    public Camera mainCamera;
    public GameManager gameManager;
    public Button goToPlanetButton;
    MapPlanets selectedPlanet;
    public MapPlanets debug_test_planet;

    public GameObject ship;

    // Use this for initialization
    void Start () {
        gameManager = GameManager.instance;
        if (false) //DEBUG
        {
            selectedPlanet = debug_test_planet;
            GoToPlanet();
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (mainCamera.orthographicSize < 3)
        {
            for (int i=0; i < planets.Length; i++)
            {
                if (planets[i].currentlySelected)
                {
                    planetHighlighter.SetActive(true);
                    goToPlanetButton.interactable = true;
                    planetHighlighter.transform.position = planets[i].transform.position;
                    selectedPlanet = planets[i];
                    i = planets.Length + 1;
                }
                else
                {
                    selectedPlanet = null;
                    planetHighlighter.SetActive(false);
                    goToPlanetButton.interactable = false;
                }
            }
        }
        else
        {
            planetHighlighter.SetActive(false);
        }
	}

    public void GoToPlanet()
    {
        Debug.Log("Moving to planet type " + (Shared.RoomTypes)selectedPlanet.resourceNum);
        //gameManager.Rooms[6].RoomType = (Shared.RoomTypes)selectedPlanet.resourceNum;
        Firebase.Database.FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/CurrentPlanet").SetValueAsync(selectedPlanet.resourceNum);
    }
}
