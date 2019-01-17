using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crew : MonoBehaviour {
    string name;
    string role;
    int skillPoints;
    float progressToNextLevel;
    Skills skills;

    public bool newCharacter;

    struct Skills
    {
        int cooking;
        int navigation;
        int medical;
        int fighting;

    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Crew(string name, string role, int[] skills)
    {
        this.name = name;
        this.role = role;
        for (int i=0; i < skills.Length; i++)
        {

        }
    }

    public string GetName()
    {
        return name;
    }

    public string GetRole()
    {
        return role;
    }

    public void ChangeRole(string newRole)
    {
        role = newRole;
    }

    public bool NextLevel()
    {
        if (progressToNextLevel > 100)
        {
            progressToNextLevel = -100;
            skillPoints += 3;
            return true;
        }
        else
            return false;
    }

    public float nextLevelProgress()
    {
        return progressToNextLevel;
    }

    
}
