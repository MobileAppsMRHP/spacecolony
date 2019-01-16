using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crew : MonoBehaviour {
    string name;
    string role;
    struct skillLevel
    {
        int cooking;
        int piloting;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Crew(string name, string role)
    {
        this.name = name;
        this.role = role;

    }

    public string getName()
    {
        return name;
    }

    public string getRole()
    {
        return role;
    }

    public void changeRole(string newRole)
    {
        role = newRole;
    }
}
