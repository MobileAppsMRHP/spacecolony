using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Crew : MonoBehaviour {

    private string name;
    private string role;
    private int skillPoints;
    private float progressToNextLevel;
    private SharedStructs.Skills skills;

    public bool newCharacter;

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
        this.skillPoints = 0;
        this.progressToNextLevel = 0;

        this.skills = new SharedStructs.Skills();

        for (int i=0; i < skills.Length; i++)
        {
            //this.skills[i] = skills[i];
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
