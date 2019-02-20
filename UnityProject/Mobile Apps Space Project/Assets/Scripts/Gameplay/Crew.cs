using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;



public class Crew : MonoBehaviour {

    public string crewName;
    public string role;
    public int skillPoints;
    public float progressToNextLevel;
    public Shared.Skills skills;

    private GameManager gameManager;

    //public bool newCharacter;

	// Use this for initialization
	void Start () {
        FirebaseDatabase.DefaultInstance.GetReference("testing-data").Child(this.crewName).SetRawJsonValueAsync(JsonUtility.ToJson(this));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Crew(string crewName, string role, Shared.Skills skills)
    {
        gameManager = GameManager.instance;
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

    public Crew(string rawJson, System.EventHandler<ValueChangedEventArgs> EventHandler)
    {
        gameManager = GameManager.instance;
        EventHandler += HandleValueChanged;
        JsonUtility.FromJsonOverwrite(rawJson, this);
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
