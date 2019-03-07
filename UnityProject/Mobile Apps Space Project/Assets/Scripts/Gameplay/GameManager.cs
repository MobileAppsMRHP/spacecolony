using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.DefaultControls;

[System.FlagsAttribute] public enum DebugFlags// : short
{
    None = 0,
    Critical = 1,
    Warning = 2,
    DatabaseOps = 4,
    CollisionOps = 8,
    option6 = 16,
    option7 = 32,
    option8 = 64,
    option9 = 128,
    option10 = 256,
    option11 = 512,
    option12 = 1024,
    option13 = 2048,
    option14 = 4096
} //16 options with short

public class GameManager : MonoBehaviour
{

    public List<Crew> CrewMembers;
    public List<Room> Rooms;
    private Dictionary<string, int> Resources; //{ get; }

    private bool IsLoading; //indicates to other things if the game is loading

    public const byte debugLevel = 255; //increase this value to log more debug messages
    //higher number includes more logs, so picking 2 includes both 1 and 2's output
    //0: no debug log messages ever
    //1: log errors only
    //2: log warnings
    //3: log info
    //4: log every database value change
    //5: 
    //6: 
    //7: 
    //255: log EEEEVERYTHING

    public const DebugFlags debugLevelFlags = DebugFlags.Critical | DebugFlags.Warning | DebugFlags.DatabaseOps;
    //add or subtract values from DebugFlags to change what gets printed, or set to short.MaxValue to print everything
    //example debugLevelFlags = DebugFlags.Critical + DebugFlags.Warning + DebugFlags.CollisionOps
    //example debugLevelFlags = short.MaxValue - DebugFlags.CollisionOps




    public RuntimePlatform running_on;

    public static GameManager instance = null; //singleton pattern

    private static DatabaseManager dbman = null;
    public CrewSpawner crewCreator;
    public RoomSpawner roomCreator;

    public string user_string = "StillLoading";

    private void Start()
    {
        DisplayLoadingScreen();
        running_on = Application.platform;
        DebugLog("Running on a " + running_on, 3);
        Authenticate();
        user_string = "User1"; //TODO: get actual from auth
        SetupResourcesList();
        LoadDatabaseValues();
        HideLoadingScreen();
    }

    // Use this for initialization
    void Awake() { //Only initializes once
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    void Update() {

    }

    public void Authenticate()
    {
        //call auth stuff here
    }

    public void SetupResourcesList()
    {
        Resources = new Dictionary<string, int>()
        {
            { "food", 0 },
            { "water", 0 },
            { "energy", 0 },
            { "money", 0 }
        };
        DebugLog("Done setting up resrouces list", 3);

    }

    public int GetResource(string key)
    {
        if (Resources.ContainsKey(key))
            return Resources[key];
        else
        {
            DebugLog("A resource " + key + " was requested that was not present in the resources list.", 2);
            return 0;
        }
    }

    public int SetResource(string key, int value)
    {
        Resources[key] = value;
        return Resources[key];
    }

    /*public void AddFood(int num)
    {
        food += num;
    }*/

    //TODO: make this actually load database values, initalize crew and resources, etc.
    public void LoadDatabaseValues()
    {
        dbman = new DatabaseManager();

        LoadRooms();
        LoadCrew();

        dbman.LoadResources();
    }

    void LoadCrew()
    {
        //crewCreator.CreateCrewMember();
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + user_string + "/Crew/").GetValueAsync().ContinueWith(task =>
       {
           if (task.IsFaulted)
           {
               // Handle the error...
               DebugLog("Data retrival error when prompting for crew data!", 1);
           }
           else if (task.IsCompleted)
           {
               foreach(DataSnapshot crewMember in task.Result.Children)
               {
                   DebugLog("Found crewmember with ID " + crewMember.Key, 4);
                   SpawnCrew(crewMember.Key /*new List<object> { crewMember.Key, crewCreator }*/);
               }
           }
           else
           {
               //The task neither completed nor failed, this shouldn't happen. Should only be reached if task is canceled?
               DebugLog("Task error when prompting for crew data", 1);
           }
       });
    }

    private void SpawnCrew(string identifier /*List<object> data*/)
    { //expected data format:
            // string           identifer string
            // CrewCreator      spawner to teleport to upon spawning
        DebugLog("Spawning crew member via dispatch...");
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            //Debug.Log("This is executed from the main thread");
            Crew newCrewMember = Instantiate(crewCreator.prefab);

            newCrewMember.SendMessage("CrewCreatorStart", identifier /*data*/);
            Debug.Log("Created crew member with ID " + identifier /*(string)data[0]*/);

            CrewMembers.Add(newCrewMember);
        });

    }
        

    void LoadRooms()
    {
        roomCreator.CreateRoom();
    }

    //TODO: make this actually display a loading screen
    public void DisplayLoadingScreen()
    {
        IsLoading = true;

    }

    //TODO: make this actually display a loading screen
    public void HideLoadingScreen()
    {
        IsLoading = false;

    }

    public static void DebugLog(string message, DebugFlags flagLevelToDisplayAt /*byte debugLevelToDisplayAt*/)
    {
        if ((debugLevelFlags & flagLevelToDisplayAt) != 0) //check if debugLevelFlags contains the flag passed in
        {
            if (flagLevelToDisplayAt == DebugFlags.Critical)
                Debug.LogError(message);
            else if(flagLevelToDisplayAt == DebugFlags.Warning)
                Debug.LogWarning(message);
            else
                Debug.Log(message);

        }
        /*if (debugLevel >= debugLevelToDisplayAt)
            if(debugLevelToDisplayAt == 1)
                Debug.LogError(message);
            else
                Debug.Log(message);*/
    }

    public static void DebugLog(string message, byte debugLevelToDisplayAt) //alternate, old version of flags system
    {
        if (debugLevel >= debugLevelToDisplayAt)
            if(debugLevelToDisplayAt == 1)
                Debug.LogError(message);
            else
                Debug.Log(message);
    }

    public static void DebugLog(string message) //overloaded
    {
        DebugLog(message, (byte)0);
    }
}
