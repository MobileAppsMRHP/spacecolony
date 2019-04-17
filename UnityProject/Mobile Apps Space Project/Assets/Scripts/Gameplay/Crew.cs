using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using System;



public class Crew : MonoBehaviour, IFirebaseTimedUpdateable, IProcessElapsedTime {

    public Room currentRoom;
    
    public string identifier = "_BLANK"; //don't change this! only fresh crew members get this changed by the code

    [System.NonSerialized] public static List<string> Possible_Names = new List<string>();
    public static float Exp_Per_Sec = 0.01f;
    public GameManager gameManager;

    private int totalUpdatesRecieved = 0;

    //private GameManager gameManager;

    //public bool newCharacter;

    [System.Serializable]
    public struct TimedDataToSerialize
    {
        public float ProgressToNextLevel;
    }

    [System.Serializable]
    public struct RoomStuff
    {
        /*private String currentRoomString;
        public string CurrentRoomStringForDB {
            get
            {
                return currentRoomString;
            }
            set
            {
                currentRoomString = value;
                DatabaseUpdateRoomData();
            }*/
        public string CurrentRoomStringForDB;
    }

    [System.Serializable]
    public struct Skills
    {
        public int SkillPoints;
        public int Skill_Cooking;
        public int Skill_Navigation;
        public int Skill_Medical;
        public int Skill_Fighting;
        public int Level;
    }

    public TimedDataToSerialize TimedData;
    public Skills SkillData;
    public RoomStuff RoomData;
    public string CrewName;



    // Use this for initialization
    public void CrewCreatorStart(string identifier)
    {
        this.identifier = identifier;
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + identifier + "/RoomData").ValueChanged += HandleRoomValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + identifier + "/SkillData").ValueChanged += HandleSkillValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + identifier + "/TimedData").ValueChanged += HandleTimedValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + identifier + "/CrewName").ValueChanged += HandleNameValueChanged;
        StartCoroutine(CrewCreatorStartMultithread());
    }

    public IEnumerator CrewCreatorStartMultithread()
    {
        GameManager.DebugLog("Waiting for crew data to be recieved before proceeding...", DebugFlags.CrewLoadingOps);
        yield return new WaitUntil(() => totalUpdatesRecieved > 0);
        GameManager.DebugLog("...crew data recieved; proceeding.", DebugFlags.CrewLoadingOps);
        GameManager.instance.AddToFirebaseTimedUpdates(this);
        GameManager.instance.AddToProcessElapsedTime(this);
        GameManager.DebugLog("Crew startup done for " + identifier, DebugFlags.CrewLoadingOps);
    }

    // Update is called once per frame
    void Update() {
        IncreaseExperience();
    }

    public static void BuildRandomNameList() //run first to build name list
    {
        GameManager.DebugLog("Building names list...", DebugFlags.CrewLoadingOps);
        List<string> temp_Possible_Names = new List<string>();
        FirebaseDatabase.DefaultInstance.GetReference("new-object-templates/possible-crew-names").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                GameManager.DebugLog("Data retrival error when prompting for possible crew names!", DebugFlags.Critical);
            }
            else if (task.IsCompleted)
            {
                string tempString = "";
                foreach (DataSnapshot possibleName in task.Result.Children)
                {
                    //GameManager.DebugLog("Found name: " + possibleName.Value.ToString(), DebugFlags.CrewLoadingOps);
                    string val = possibleName.Value.ToString();
                    temp_Possible_Names.Add(val);
                    tempString += val + "|";
                }
                Possible_Names = temp_Possible_Names;
                GameManager.DebugLog("Name List done building, items: " + Possible_Names.Count + " | " + tempString/* + " | " + JsonUtility.ToJson(Possible_Names)*/, DebugFlags.CrewLoadingOps);
                //GameManager.DebugLog("Name 1 " + Possible_Names[1]);
                /*foreach (string thisName in Possible_Names)
                {
                    GameManager.DebugLog("Names list item : " + thisName, DebugFlags.CrewLoadingOps);
                }*/
            }
            else
            {
                //The task neither completed nor failed, this shouldn't happen. Should only be reached if task is canceled?
                GameManager.DebugLog("Task error when prompting for possible crew names", DebugFlags.Critical);
            }
        });
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        totalUpdatesRecieved++;
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote crew member " + this.name + " with JSON from database: " + json, DebugFlags.DatabaseOps);
        JsonUtility.FromJsonOverwrite(json, this);
    }



    public void FreshCrewSetup()
    {
        //few things here need initial values because they are set by the prefab. to give initial values, edit the prefab.
        //setup unique crew identifier
        GameManager.DebugLog("FRESH Crew Coroutine Start", DebugFlags.CrewLoadingOps);
        StartCoroutine(SetUpAndWriteFreshCrew());
    }

    IEnumerator SetUpAndWriteFreshCrew()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc); //from https://answers.unity.com/questions/417939/how-can-i-get-the-time-since-the-epoch-date-in-uni.html
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        identifier = "" + cur_time;

        if (Possible_Names.Count == 0)
        {
            GameManager.DebugLog("Waiting for names list to build...", DebugFlags.CrewLoadingOps);
            yield return new WaitUntil(() => Possible_Names.Count != 0);
            GameManager.DebugLog("Names list done building. Continuing with name setting. Length: " + Possible_Names.Count, DebugFlags.CrewLoadingOps);
        }
        System.Random rand = new System.Random();
        CrewName = Possible_Names[rand.Next(Possible_Names.Count)];

        //write self to database
        GameManager.DebugLog("Writing FRESH crew with name " + CrewName + " and id " + identifier + " to database...", DebugFlags.CrewLoadingOps);
        yield return FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + this.identifier + "CrewName").SetRawJsonValueAsync(JsonUtility.ToJson(CrewName));

        GameManager.DebugLog("FRESH crew setup done for " + identifier, DebugFlags.CrewLoadingOps);
        CrewCreatorStart(identifier); //run regular setup
    }

    /*public string GetName()
    {
        return crewName;
    }*/

    public bool NextLevel()
    {
        if (GetComponent<DragAndDrop>().inRoom && TimedData.ProgressToNextLevel > 100)
        {
            TimedData.ProgressToNextLevel -= 100;
            SkillData.SkillPoints += 3;
            SkillData.Level++;
            DatabaseUpdateSkillsData();
            GameManager.DebugLog("Crew " + identifier + " leveled up!", DebugFlags.GeneralInfo);
            return true;
        }
        else
            return false;
    }

    public void IncreaseSkill(int skillNum)
    {
        if (SkillData.SkillPoints > 0)
        {
            switch (skillNum)
            {
                case 0:
                    SkillData.Skill_Cooking++;
                    break;
                case 1:
                    SkillData.Skill_Navigation++;
                    break;
                case 2:
                    SkillData.Skill_Medical++;
                    break;
                case 3:
                    SkillData.Skill_Fighting++;
                    break;
            }
            SkillData.SkillPoints--;
            DatabaseUpdateSkillsData();
        }
    }

    void IncreaseExperience()
    {
        ProcessTime(Time.deltaTime);
    }

    public void ProcessTime(float deltaTime)
    {
        if (currentRoom != null)
        {
            float progress = Exp_Per_Sec * deltaTime;
            TimedData.ProgressToNextLevel += progress;
            if (deltaTime > Shared.ProcessElapsedTime_ConsiderLoggedOff)
            {
                GameManager.DebugLog("[ElapsedTime] " + deltaTime + " seconds passed, causing crew " + identifier + " to increase ProgressToNextLevel by " + progress, DebugFlags.ElapsedTime);
                FirebaseUpdate(false);
            }
            
        }
    }

    public void FirebaseUpdate(bool wasTimedUpdate)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            //Debug.Log("This is executed from the main thread");
            string json = JsonUtility.ToJson(TimedData);
            FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + this.identifier + "/TimedDataToSerialize").SetRawJsonValueAsync(json);
            if (wasTimedUpdate)
                GameManager.DebugLog("[TimedUpdate] Updated crew " + identifier + " TimedData database contents with " + json, DebugFlags.DatabaseOpsOnTimer);
            else
                GameManager.DebugLog("[>TriggeredUpdate] Updated crew " + identifier + " TimedData database contents with " + json, DebugFlags.DatabaseOps);
        });
    }

    public void DatabaseUpdateRoomData()
    {
        string json = JsonUtility.ToJson(RoomData);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + this.identifier + "/RoomData").SetRawJsonValueAsync(json);
        GameManager.DebugLog("[TimedUpdate] Updated crew " + identifier + " RoomData database contents with " + json, DebugFlags.DatabaseOpsOnTimer);
    }

    public void DatabaseUpdateSkillsData()
    {
        string json = JsonUtility.ToJson(SkillData);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + this.identifier + "/SkillData").SetRawJsonValueAsync(json);
        GameManager.DebugLog("[TimedUpdate] Updated crew " + identifier + " SkillData database contents with " + json, DebugFlags.DatabaseOpsOnTimer);
    }

    public void MoveCrewBasedOnString(string roomToMoveTo)
    {
        Room roomToMoveCrewTo = null;
        foreach (var item in gameManager.Rooms)
        {
            if (item.data.RoomUniqueIdentifierForDB.Equals(roomToMoveTo))
            {
                roomToMoveCrewTo = item;
                break;
            }
        }
        if(roomToMoveCrewTo == null) //if still null, error
        {
            GameManager.DebugLog("Tried to move crew '" + identifier + "' to a room that doesn't exist ('" + roomToMoveTo + "')", DebugFlags.Warning);
            return;// null;
        }
        transform.position = roomToMoveCrewTo.GetComponent<Transform>().position;
        GameManager.DebugLog("Successfully moved crew '" + identifier + "' to room '" + roomToMoveTo + "'", DebugFlags.CollisionOps);
        DatabaseUpdateRoomData();
    }
}
