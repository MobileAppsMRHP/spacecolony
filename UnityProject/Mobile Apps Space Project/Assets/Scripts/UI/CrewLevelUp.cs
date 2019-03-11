using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewLevelUp : MonoBehaviour {
    public Crew selectedCrew;
    int crewNum;
    public GameManager gameManager;
	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
        selectedCrew = gameManager.CrewMembers[0];
        crewNum = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SwitchCrew(bool right)
    {
        if (right)
        {
            crewNum = crewNum == gameManager.CrewMembers.Count - 1 ? 0 : crewNum + 1;
        }
        else
        {
            crewNum = crewNum == 0 ? gameManager.CrewMembers.Count - 1 : crewNum - 1;
        }
        selectedCrew = gameManager.CrewMembers[crewNum];
    }

    public void IncreaseStat(int num)
    {
        selectedCrew.IncreaseSkill(num);
    }
}
