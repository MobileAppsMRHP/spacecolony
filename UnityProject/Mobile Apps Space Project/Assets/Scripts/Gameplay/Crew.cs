using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Crew : MonoBehaviour {

    public string crewName;
    public string role;
    public int skillPoints;
    public float progressToNextLevel;
    public Shared.Skills skills;

    Room currentRoom;

    //public bool newCharacter;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Crew(string crewName, string role, Shared.Skills skills)
    {
        this.crewName = crewName;
        this.role = role;
        this.skillPoints = 0;
        this.progressToNextLevel = 0;

        //this.skills = skills; // new SharedStructs.Skills();

        /*for (int i=0; i < skills.Length; i++)
        {
            //this.skills[i] = skills[i];
        }*/
    }

    public string GetName()
    {
        return crewName;
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

    public float NextLevelProgress()
    {
        return progressToNextLevel;
    }

    public string Serialize()
    { //rob test method
        return JsonUtility.ToJson(this);
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    public void ChangeCurrentRoom(Room newRoom)
    {
        currentRoom = newRoom;
    }
    
}
