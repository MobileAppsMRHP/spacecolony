using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;



public class Crew : MonoBehaviour {

    public string crewName;
    public string role;
    public int skillPoints;
    public float progressToNextLevel;

    public int cooking;
    public int navigation;
    public int medical;
    public int fighting;

    public string identifier = "_BLANK"; //don't change this! only fresh crew members get this changed by the code

    //private GameManager gameManager;

    //public bool newCharacter;

	// Use this for initialization
	public void CrewCreatorStart (string identifier /*List<object> data*/)
    {
        //this.identifier = (string)data[0];
        this.identifier = identifier;
        //CrewSpawner spawner = (CrewSpawner)data[1];
        //transform.position = Vector3.MoveTowards(transform.position, spawner.transform.position, 1); //go to spawner
        //FirebaseDatabase.DefaultInstance.GetReference("testing-data").Child(this.crewName).SetRawJsonValueAsync(JsonUtility.ToJson(this));
        GameManager.DebugLog("I exist! " + identifier);
        //FreshCrewSetup();
        //SerializeWrite();
        
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + identifier).ValueChanged += HandleValueChanged;
	}
	
	// Update is called once per frame
	void Update () {
        IncreaseExperience();
	}

    /*public Crew(string crewName, string role, Shared.Skills skills)
    {
        gameManager = GameManager.instance;
        this.crewName = crewName;
        this.role = role;
        this.skillPoints = 0;
        this.progressToNextLevel = 0;

        //this.skills = skills; // new SharedStructs.Skills();

        
    }*/

    /*public Crew(string rawJson, System.EventHandler<ValueChangedEventArgs> EventHandler)
    {
        gameManager = GameManager.instance;
        EventHandler += HandleValueChanged;
        JsonUtility.FromJsonOverwrite(rawJson, this);
    }*/

    public void FreshCrewSetup()
    {
        //setup unique crew identifier
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc); //from https://answers.unity.com/questions/417939/how-can-i-get-the-time-since-the-epoch-date-in-uni.html
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        identifier = "" + cur_time;


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
        if (GetComponent<DragAndDrop>().inRoom && progressToNextLevel > 100)
        {
            progressToNextLevel -= 100;
            skillPoints += 3;
            return true;
        }
        else
            return false;
    }

    public void IncreaseSkill(int skillNum)
    {
        switch (skillNum)
        {
            case 1:
                cooking++;
                break;
            case 2:
                navigation++;
                break;
            case 3:
                medical++;
                break;
            case 4:
                fighting++;
                break;
        }
    }

    void IncreaseExperience()
    {
        if (GetComponent<DragAndDrop>().inRoom)
        {
            progressToNextLevel += 0.01f * Time.deltaTime;
        }
    }


    public string Serialize()
    { //rob test method
        return JsonUtility.ToJson(this);
    }

    public void SerializeWrite()
    {
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew").Child(identifier).SetRawJsonValueAsync(JsonUtility.ToJson(this));
    }

    public void LoadCrew(DatabaseManager dbman)
    {
        //FirebaseDatabase.DefaultInstance.GetReference(dbman.user_string).Child("Crew").ValueChanged += HandleValueChanged;

        //var query = dbman.instance.GetReference(dbman.user_string).Child("Crew").GetValueAsync()
        dbman.instance.GetReference(GameManager.instance.user_string).Child("Crew").GetValueAsync()
           .ContinueWith(task => {
               if (task.IsFaulted)
               {
                    // Handle the error...
                    GameManager.DebugLog("Data retrival error when prompting for Crew data for user " + GameManager.instance.user_string, 1);
               }
               else if (task.IsCompleted)
               {
                   Debug.Log("got Crew members data for " + GameManager.instance.user_string);
                   foreach (var item in task.Result.Children)
                   {
                       //Instantiate(CrewMemberPrefab, )
                       //System.EventHandler<ValueChangedEventArgs> EventHandler = FirebaseDatabase.DefaultInstance.GetReference(dbman.user_string).Child("Crew").ValueChanged;
                       //GameManager.instance.CrewMembers.Add(new Crew(item.GetRawJsonValue(), EventHandler));
                   }
               }
               else
               {
                    //The task neither completed nor failed, this shouldn't happen. Should only be reached if task is canceled?
                    GameManager.DebugLog("Task error! Crew data request", 1);
               }
           });



        //ValueChanged += DatabaseValueChanged;
    }


    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote crew member " + this.name + " with JSON from database: " + json, 4);
        JsonUtility.FromJsonOverwrite(json, this);
    }
    

}
