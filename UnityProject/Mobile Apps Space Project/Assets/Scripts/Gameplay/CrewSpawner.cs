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

    public void CreateCrewMember()
    {
        Crew newCrewMember = Instantiate(prefab, this.transform); //create new crew member prefab at the prefab
    }
}
