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

    //private bool IsLoading; //indicates to other things if the game is loading

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

    public ResourceManager resourceManager;
    public CrewSpawner crewCreator;
    public RoomSpawner roomCreator;
    protected static UserAuthentication auth;

    public string user_string = "StillLoading";

    private void Start()
    {
        DEBUG_WriteNewCrewTemplate();
        DEBUG_WriteNewRoomTemplate();
        DisplayLoadingScreen();
        running_on = Application.platform;
        DebugLog("Running on a " + running_on, 3);
        user_string = Authenticate();
        //user_string = "User1"; //TODO: get actual from auth
        LoadDatabaseValues();
        HideLoadingScreen();

        CreateFreshCrewMember();
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

    public string Authenticate()
    {
        auth = gameObject.AddComponent<UserAuthentication>();
        Firebase.Auth.Credential token= auth.getCredential();
        print(token);
        return "User1";
    }

    public void LoadDatabaseValues()
    {
        LoadResources();
        LoadRooms(); //load rooms first so crew know where to go
        LoadCrew();
    }

    void LoadCrew()
    {
        Crew.BuildRandomNameList();
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

    private void SpawnCrew(string identifier)
    {
        DebugLog("Spawning crew member via dispatch...");
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            //Debug.Log("This is executed from the main thread");
            Crew newCrewMember = Instantiate(crewCreator.prefab);

            newCrewMember.SendMessage("CrewCreatorStart", identifier);
            Debug.Log("Created crew member with ID " + identifier);

            CrewMembers.Add(newCrewMember);
            newCrewMember.transform.SetParent(crewCreator.transform);
        });

    }

    public void CreateFreshCrewMember()
    {
        DebugLog("Spawning fresh member via dispatch...");
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            //Debug.Log("This is executed from the main thread");
            Crew newCrewMember = Instantiate(crewCreator.prefab);

            newCrewMember.SendMessage("FreshCrewSetup");
            Debug.Log("Created fresh crew member");

            CrewMembers.Add(newCrewMember);
            newCrewMember.transform.SetParent(crewCreator.transform);
        });
    }

    void DEBUG_WriteNewCrewTemplate() //This method overwrites the template in Firebase with the current fresh prefab's data
    {
        FirebaseDatabase.DefaultInstance.GetReference("new-object-templates/crew").SetRawJsonValueAsync(JsonUtility.ToJson(crewCreator.prefab));
    }

    void DEBUG_WriteNewRoomTemplate() //This method overwrites the template in Firebase with the current fresh prefab's data
    {
        FirebaseDatabase.DefaultInstance.GetReference("new-object-templates/room").SetRawJsonValueAsync(JsonUtility.ToJson(roomCreator.prefab));
    }


    void LoadRooms()
    {
        //roomCreator.CreateRoom();
    }

    //TODO: make this actually load resources.
    void LoadResources()
    {
        resourceManager = new ResourceManager(); //loading handled by its startup
    }

    //TODO: make this actually display a loading screen
    public void DisplayLoadingScreen()
    {
        //IsLoading = true;

    }

    //TODO: make this actually display a loading screen
    public void HideLoadingScreen()
    {
        //IsLoading = false;
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
