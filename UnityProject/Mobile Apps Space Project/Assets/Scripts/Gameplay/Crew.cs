using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using System;



public class Crew : MonoBehaviour {

    public string crewName;
    public int skillPoints;
    public float progressToNextLevel;

    public int cooking;
    public int navigation;
    public int medical;
    public int fighting;
    public int level;

    public Room currentRoom;
    public string CurrentRoomStringForDB = "NO_ROOM";

    public string identifier = "_BLANK"; //don't change this! only fresh crew members get this changed by the code

    public static List<string> Possible_Names = new List<string>();

    //private GameManager gameManager;

    //public bool newCharacter;

    // Use this for initialization
    public void CrewCreatorStart(string identifier)
    {
        this.identifier = identifier;
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + identifier).ValueChanged += HandleValueChanged;
        GameManager.DebugLog("Crew startup done for " + identifier);
    }

    // Update is called once per frame
    void Update() {
        IncreaseExperience();
    }

    public static void BuildRandomNameList() //run first to build name list
    {
        GameManager.DebugLog("Building names list...");
        List<string> temp_Possible_Names = new List<string>();
        FirebaseDatabase.DefaultInstance.GetReference("new-object-templates/possible-crew-names").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                GameManager.DebugLog("Data retrival error when prompting for possible crew names!", 1);
            }
            else if (task.IsCompleted)
            {
                string tempString = "";
                foreach (DataSnapshot possibleName in task.Result.Children)
                {
                    //GameManager.DebugLog("Found name: " + possibleName.Value.ToString());
                    string val = possibleName.Value.ToString();
                    temp_Possible_Names.Add(val);
                    tempString += val + "|";
                }
                Possible_Names = temp_Possible_Names;
                GameManager.DebugLog("Name List done building, items: " + Possible_Names.Count + " | " + tempString/* + " | " + JsonUtility.ToJson(Possible_Names)*/);
                //GameManager.DebugLog("Name 1 " + Possible_Names[1]);
                /*foreach (string thisName in Possible_Names)
                {
                    GameManager.DebugLog("Names list item : " + thisName);
                }*/
            }
            else
            {
                //The task neither completed nor failed, this shouldn't happen. Should only be reached if task is canceled?
                GameManager.DebugLog("Task error when prompting for possible crew names", 1);
            }
        });
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote crew member " + this.name + " with JSON from database: " + json, 4);
        JsonUtility.FromJsonOverwrite(json, this);
    }

    public void FreshCrewSetup()
    {
        //few things here need initial values because they are set by the prefab. to give initial values, edit the prefab.
        //setup unique crew identifier
        GameManager.DebugLog("FRESH Crew Coroutine Start");
        StartCoroutine(SetUpAndWriteFreshCrew());
    }

    IEnumerator SetUpAndWriteFreshCrew()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc); //from https://answers.unity.com/questions/417939/how-can-i-get-the-time-since-the-epoch-date-in-uni.html
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        identifier = "" + cur_time;

        if (Possible_Names.Count == 0)
        {
            Debug.Log("Waiting for names list to build...");
            yield return new WaitUntil(() => Possible_Names.Count != 0);
            Debug.Log("Names list done building. Continuing with name setting. Length: " + Possible_Names.Count);
        }
        System.Random rand = new System.Random();
        this.crewName = Possible_Names[rand.Next(Possible_Names.Count)];

        //write self to database
        GameManager.DebugLog("Writing FRESH crew with name " + crewName + " and id " + identifier + " to database...");
        yield return FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + this.identifier).SetRawJsonValueAsync(JsonUtility.ToJson(this));
        GameManager.DebugLog("FRESH crew setup done for " + identifier);
        CrewCreatorStart(identifier); //run regular setup
    }

    public string GetName()
    {
        return crewName;
    }

    public bool NextLevel()
    {
        if (GetComponent<DragAndDrop>().inRoom && progressToNextLevel > 100)
        {
            progressToNextLevel -= 100;
            skillPoints += 3;
            level++;
            return true;
        }
        else
            return false;
    }

    public void IncreaseSkill(int skillNum)
    {
        if (skillPoints > 0)
        {
            switch (skillNum)
            {
                case 0:
                    cooking++;
                    break;
                case 1:
                    navigation++;
                    break;
                case 2:
                    medical++;
                    break;
                case 3:
                    fighting++;
                    break;
            }
            skillPoints--;
        }
    }

    void IncreaseExperience()
    {
        if (GetComponent<DragAndDrop>().inRoom)
        {
            progressToNextLevel += 0.01f * Time.deltaTime;
        }
    }
}
