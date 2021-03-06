﻿using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using System;



public class Crew : MonoBehaviour, IFirebaseTimedUpdateable, IProcessElapsedTime {

    public Room currentRoom;
    public bool DoCollisions = false;

    public string identifier = "_BLANK"; //don't change this! only fresh crew members get this changed by the code

    [System.NonSerialized] public static List<string> Possible_Names = new List<string>();
    public static float Exp_Per_Sec = 0.01f;

    [System.Serializable] //contains all of the data to be serialized in the database. broken down into chunks so that the entire object isn't being overwritten be one thing being changed
    public struct Data
    {
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
    }

    public Data AllData; //access data contents through here

    public string CrewName //convenience
    {get {return AllData.CrewName;} set {AllData.CrewName = value;}}

    private int totalUpdatesRecieved = 0;

    public IEnumerator CrewCreatorStartMultithread(string identifier)
    {
        this.identifier = identifier;
        //attach data listeners to load in the data
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + identifier + "/RoomData").ValueChanged += HandleRoomValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + identifier + "/SkillData").ValueChanged += HandleSkillValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + identifier + "/TimedData").ValueChanged += HandleTimedValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + identifier + "/CrewName").ValueChanged += HandleNameValueChanged;
        GameManager.DebugLog("Spawning crew - Waiting for this crew's data to be received before proceeding...", DebugFlags.CrewLoadingOps);
        yield return new WaitUntil(() => totalUpdatesRecieved >= 4); //4 pieces of data to get. need to wait for data to load so the database contents aren't overwritten
        GameManager.DebugLog("...crew data received; proceeding.", DebugFlags.CrewLoadingOps);
        GameManager.instance.AddToFirebaseTimedUpdates(this); //write experience increases on a timer
        GameManager.instance.AddToProcessElapsedTime(this); //allow elapsed time to be calculated on this crew member
        //UnityEngine.Assertions.Assert.IsTrue("NO_ROOM".Equals(GameManager.instance.crewCreator.prefab.AllData.RoomData.CurrentRoomStringForDB)); //ensure that the prefab has blank room value
        yield return new WaitUntil(() => !AllData.RoomData.CurrentRoomStringForDB.Equals("NO_ROOM")); //wait until room data loads in on this thread
        DoCollisions = true; //allow collisions to cause the crew member to move rooms now
        StartCoroutine(MoveCrewBasedOnString(AllData.RoomData.CurrentRoomStringForDB)); //move the crew to the room they're supposed to be in
        GameManager.DebugLog("Crew startup done for " + identifier, DebugFlags.CrewLoadingOps);
    }

    // Update is called once per frame
    void Update() {
        IncreaseExperience();
        if (GetComponent<DragAndDrop>().inRoom && AllData.TimedData.ProgressToNextLevel > 100) //process level up
        {
            AllData.TimedData.ProgressToNextLevel -= 100;
            AllData.SkillData.SkillPoints += 3;
            AllData.SkillData.Level++;
            DatabaseUpdateSkillsData();
            GameManager.DebugLog("Crew " + identifier + " leveled up!", DebugFlags.GeneralInfo);
        }
    }

    public static void BuildRandomNameList() //run to build name list for new crew before spawning new crew
    {
        GameManager.DebugLog("Building names list...", DebugFlags.CrewLoadingOps);
        List<string> temp_Possible_Names = new List<string>();
        FirebaseDatabase.DefaultInstance.GetReference("new-object-templates/possible-crew-names").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                GameManager.DebugLog("Data retrieval error when prompting for possible crew names!", DebugFlags.Critical);
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
                GameManager.DebugLog("Name List done building, items: " + Possible_Names.Count + " | " + tempString, DebugFlags.CrewLoadingOps);
            }
            else
            {
                //The task neither completed nor failed, this shouldn't happen. Should only be reached if task is canceled?
                GameManager.DebugLog("Task error when prompting for possible crew names", DebugFlags.Critical);
            }
        });
    }

    void HandleRoomValueChanged(object sender, ValueChangedEventArgs args)
    {   //deal with Data's room subset changing value
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote crew member " + this.CrewName + " (" + identifier + ") ROOM data with JSON from database: " + json, DebugFlags.DatabaseOps);
        object boxedDataCloneForJsonUtility = AllData.RoomData; //needs special boxing because https://docs.unity3d.com/ScriptReference/EditorJsonUtility.FromJsonOverwrite.html
        JsonUtility.FromJsonOverwrite(json, boxedDataCloneForJsonUtility);
        AllData.RoomData = (Data.RoomStuff)boxedDataCloneForJsonUtility;
        //this.MoveCrewBasedOnString(AllData.RoomData.CurrentRoomStringForDB);
        totalUpdatesRecieved++;
    }

    void HandleSkillValueChanged(object sender, ValueChangedEventArgs args)
    {//deal with Data's skill subset changing value
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote crew member " + this.CrewName + " (" + identifier + ") SKILL data with JSON from database: " + json, DebugFlags.DatabaseOps);
        object boxedDataCloneForJsonUtility = AllData.SkillData; //needs special boxing because https://docs.unity3d.com/ScriptReference/EditorJsonUtility.FromJsonOverwrite.html
        JsonUtility.FromJsonOverwrite(json, boxedDataCloneForJsonUtility);
        AllData.SkillData = (Data.Skills)boxedDataCloneForJsonUtility;
        totalUpdatesRecieved++;
    }

    void HandleTimedValueChanged(object sender, ValueChangedEventArgs args)
    {//deal with Data's TimedData subset changing value
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote crew member " + this.CrewName + " (" + identifier + ") TIMED data with JSON from database: " + json, DebugFlags.DatabaseOps);
        object boxedDataCloneForJsonUtility = AllData.TimedData; //needs special boxing because https://docs.unity3d.com/ScriptReference/EditorJsonUtility.FromJsonOverwrite.html
        JsonUtility.FromJsonOverwrite(json, boxedDataCloneForJsonUtility);
        AllData.TimedData = (Data.TimedDataToSerialize)boxedDataCloneForJsonUtility;
        totalUpdatesRecieved++;
    }

    void HandleNameValueChanged(object sender, ValueChangedEventArgs args)
    {   //deal with Data's standalone name value changing
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote crew member " + this.CrewName + " (" + identifier + ") NAME data with JSON from database: " + json, DebugFlags.DatabaseOps);
        AllData.CrewName = json;
        totalUpdatesRecieved++;
    }

    public IEnumerator SetUpAndWriteFreshCrew()
    {
        GameManager.DebugLog("FRESH Crew Coroutine Start", DebugFlags.CrewLoadingOps);
        
        //create unique identifier based off of epoch time stamp
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc); //from https://answers.unity.com/questions/417939/how-can-i-get-the-time-since-the-epoch-date-in-uni.html
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        identifier = "" + cur_time;

        //build names list if needed
        if (Possible_Names.Count == 0)
        {
            GameManager.DebugLog("Waiting for names list to build...", DebugFlags.CrewLoadingOps);
            yield return new WaitUntil(() => Possible_Names.Count != 0);
            GameManager.DebugLog("Names list done building. Continuing with name setting. Length: " + Possible_Names.Count, DebugFlags.CrewLoadingOps);
        }
        //select a random name for the crew
        System.Random rand = new System.Random();
        CrewName = Possible_Names[rand.Next(Possible_Names.Count)];

        //transform.position = GameManager.instance.startRoom.transform.position;

        //teleport to new crew room. wait for this to complete.
        yield return MoveCrewBasedOnString("Room_Debug_Empty");

        //write self to database
        GameManager.DebugLog("Writing FRESH crew with name " + CrewName + " and id " + identifier + " to database...", DebugFlags.CrewLoadingOps);
        yield return FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + this.identifier + "/").SetRawJsonValueAsync(JsonUtility.ToJson(AllData));
        //yield return FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + this.identifier + "/CrewName").SetRawJsonValueAsync(JsonUtility.ToJson(CrewName));
        GameManager.DebugLog("...FRESH crew setup done for " + identifier, DebugFlags.CrewLoadingOps);
        //CrewCreatorStart(identifier); //run regular setup
        StartCoroutine(CrewCreatorStartMultithread(identifier));
        
    }

    public void IncreaseSkill(int skillNum)
    {   //handle skill increase from button press
        if (AllData.SkillData.SkillPoints > 0)
        {
            switch (skillNum)
            {
                case 0:
                    AllData.SkillData.Skill_Cooking++;
                    break;
                case 1:
                    AllData.SkillData.Skill_Navigation++;
                    break;
                case 2:
                    AllData.SkillData.Skill_Medical++;
                    break;
                case 3:
                    AllData.SkillData.Skill_Fighting++;
                    break;
            }
            AllData.SkillData.SkillPoints--;
            DatabaseUpdateSkillsData();
        }
    }

    public void IncreaseExperience()
    {   //handle xp increase based off of time difference from last frame
        ProcessTime(Time.deltaTime);
    }

    public void ProcessTime(float deltaTime)
    {   //process xp gains. used for both when loaded and elapsed time since last log on
        if (currentRoom != null && !AllData.RoomData.CurrentRoomStringForDB.Equals("Room_Debug_Empty")) //needs to be in a room that isn't the starter room to gain xp
        {
            float progress = Exp_Per_Sec * deltaTime;
            AllData.TimedData.ProgressToNextLevel += progress; //bump progress
            if (deltaTime > Shared.ProcessElapsedTime_ConsiderLoggedOff) //if significant time passed, consider it a timed update
            {
                GameManager.DebugLog("[ElapsedTime] " + deltaTime + " seconds passed, causing crew " + identifier + " to increase ProgressToNextLevel by " + progress, DebugFlags.ElapsedTime);
                FirebaseUpdate(false); //manually push timed data change to database
            }
            
        }
    }

    public void FirebaseUpdate(bool wasTimedUpdate)
    {   //push timed data changes to database
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            //Debug.Log("This is executed from the main thread");
            string json = JsonUtility.ToJson(AllData.TimedData);
            FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + this.identifier + "/TimedData").SetRawJsonValueAsync(json);
            if (wasTimedUpdate)
                GameManager.DebugLog("[TimedUpdate] Updated crew " + identifier + " TimedData database contents with " + json, DebugFlags.DatabaseOpsOnTimer);
            else
                GameManager.DebugLog("[>TriggeredUpdate] Updated crew " + identifier + " TimedData database contents with " + json, DebugFlags.DatabaseOps);
        });
    }

    public void DatabaseUpdateRoomData()
    {   //push room data changes to database
        string json = JsonUtility.ToJson(AllData.RoomData);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + this.identifier + "/RoomData").SetRawJsonValueAsync(json);
        GameManager.DebugLog("[TimedUpdate] Updated crew " + identifier + " RoomData database contents with " + json, DebugFlags.DatabaseOpsOnTimer);
    }

    public void DatabaseUpdateSkillsData()
    {   //push skill changes to database
        string json = JsonUtility.ToJson(AllData.SkillData);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Crew/" + this.identifier + "/SkillData").SetRawJsonValueAsync(json);
        GameManager.DebugLog("[TimedUpdate] Updated crew " + identifier + " SkillData database contents with " + json, DebugFlags.DatabaseOpsOnTimer);
    }

    public IEnumerator MoveCrewBasedOnString(string roomStringToMoveTo)
    {   //teleport a crew member to a room in attempt to cause the collision code to add them to that room
        Room roomToMoveCrewTo = null;
        if (GameManager.instance.Rooms == null) //wait for room data if needed
        {
            Debug.Log("Waiting for Rooms to be loaded to move crew member to " + roomStringToMoveTo + "...");
            yield return new WaitUntil(() => GameManager.instance.Rooms != null);
            GameManager.DebugLog("...Rooms loaded. Moving.");
        }
        foreach (var item in GameManager.instance.Rooms)//search rooms list for the room in question
        {
            if (item.RoomUniqueIdentifierForDB.Equals(roomStringToMoveTo))
            {
                roomToMoveCrewTo = item;
                break;
            }
        }
        if(roomToMoveCrewTo == null) //if still null, error
        {
            GameManager.DebugLog("Tried to teleport crew '" + identifier + "' to a room that doesn't exist ('" + roomStringToMoveTo + "')", DebugFlags.Warning);
            yield break;// null;
        }
        transform.position = roomToMoveCrewTo.GetComponent<Transform>().position;
        GameManager.DebugLog("Successfully teleported crew '" + identifier + "' to room '" + roomStringToMoveTo + "'", DebugFlags.CollisionOps);
        //DatabaseUpdateRoomData();
    }
}
