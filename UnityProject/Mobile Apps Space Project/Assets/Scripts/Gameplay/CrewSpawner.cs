using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewSpawner : MonoBehaviour {

    public Crew prefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateCrewMember(string identifier)
    {
        Crew newCrewMember = Instantiate(prefab); //create new crew member prefab at the spawner

        newCrewMember.SendMessage("CrewCreatorStart", identifier);
        Debug.Log("Created crew member with ID " + identifier);

        GameManager.instance.CrewMembers.Add(newCrewMember);
    }

    public void CreateFreshCrewMember()
    {
        Crew newCrewMember = Instantiate(prefab, this.transform); //create new crew member prefab at the spawner
        GameManager.instance.CrewMembers.Add(newCrewMember);
    }
}
