using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrewLevelUp : MonoBehaviour {
    public Crew selectedCrew;
    public int crewNum;
    public GameManager gameManager;
    public List<Text> crewInfo;
    public Image experienceBar;
    public Image crewImage;
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
        crewInfo[6].text = selectedCrew.level.ToString();
        crewImage.sprite = selectedCrew.GetComponent<SpriteRenderer>().sprite;
        for (int i=0; i<gameManager.CrewMembers.Count; i++)
        {
            gameManager.CrewMembers[i].GetComponent<DragAndDrop>().selected = false;
        }
        if (selectedCrew.skillPoints == 0)
        {
            //change button color or disable button
            //change a text to show lack of skill points
        }
        //show progress to next level of crew
        experienceBar.transform.localScale = new Vector3(selectedCrew.progressToNextLevel, 1, 1);
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
