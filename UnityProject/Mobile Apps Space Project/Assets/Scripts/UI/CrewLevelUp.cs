using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrewLevelUp : MonoBehaviour {
    public Crew selectedCrew;
    public int crewNum;
    public GameManager gameManager;
    public List<Text> crewInfo;
	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
        selectedCrew = gameManager.CrewMembers[0];
        crewNum = 0;
	}
	
	// Update is called once per frame
	void Update () {
        crewInfo[0].text = selectedCrew.crewName;
        crewInfo[1].text = selectedCrew.cooking.ToString();
        crewInfo[2].text = selectedCrew.navigation.ToString();
        crewInfo[3].text = selectedCrew.medical.ToString();
        crewInfo[4].text = selectedCrew.fighting.ToString();
        crewInfo[5].text = selectedCrew.skillPoints.ToString();
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
        //Debug.Log("changed crew to " + selectedCrew.crewName);
    }

    public void IncreaseStat(int num)
    {
        selectedCrew.IncreaseSkill(num);
    }
}
