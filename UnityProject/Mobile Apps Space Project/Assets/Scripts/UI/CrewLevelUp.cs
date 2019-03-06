using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewLevelUp : MonoBehaviour {
    public GameObject gameManager;
    public Crew selectedCrew;
    int crewNum;
	// Use this for initialization
	void Start () {
        selectedCrew = gameManager.GetComponent<GameManager>().CrewMembers[0];
        crewNum = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SwitchCrew(bool right)
    {
        if (right)
        {
            crewNum = crewNum == gameManager.GetComponent<GameManager>().CrewMembers.Count - 1 ? 0 : crewNum + 1;
        }
        else
        {
            crewNum = crewNum == 0 ? gameManager.GetComponent<GameManager>().CrewMembers.Count - 1 : crewNum - 1;
        }
        selectedCrew = gameManager.GetComponent<GameManager>().CrewMembers[crewNum];
    }

    public void IncreaseStat(int num)
    {
        selectedCrew.IncreaseSkill(num);
    }
}
