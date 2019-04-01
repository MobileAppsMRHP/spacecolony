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
    Auth = 16,
    DatabaseOpsOnTimer = 32,
    GeneralInfo = 64,
    CrewLoadingOps = 128,
    Resources = 256,
    option11 = 512,
    option12 = 1024,
    option13 = 2048,
    option14 = 4096
} //16 options with short

public class GameManager : MonoBehaviour
{

    public List<Crew> CrewMembers;
    public List<Room> Rooms;
    //private Dictionary<string, int> Resources; //{ get; }

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

    public const DebugFlags debugLevelFlags = DebugFlags.Critical | DebugFlags.Warning | DebugFlags.DatabaseOps | DebugFlags.Resources;
    //add or subtract values from DebugFlags to change what gets printed, or set to short.MaxValue to print everything
    //example debugLevelFlags = DebugFlags.Critical + DebugFlags.Warning + DebugFlags.CollisionOps

    public RuntimePlatform running_on;

    public static GameManager instance = null; //singleton pattern

    public ResourceManager resourceManager;
    public CrewSpawner crewCreator;
    public RoomSpawner roomCreator;
    protected static UserAuthentication auth;
    private List<IFirebaseTimedUpdateable> toFirebasePush;

    public bool isDoneLoading = false;

    public string user_string = "StillLoading";

    private void Start()
    {
        PrintEnabledDebugs();
        StartCoroutine("DebugDelayedStart");

        DisplayLoadingScreen();

        toFirebasePush = new List<IFirebaseTimedUpdateable>();
        running_on = Application.platform;
        DebugLog("Running on a " + running_on, DebugFlags.GeneralInfo);
        user_string = Authenticate();
        //user_string = "User1"; //TODO: get actual from auth

        resourceManager = new ResourceManager();
        //resourceManager.DEBUG_SetupResourcesList();
        

        LoadCrew();



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

    System.Collections.IEnumerator DebugDelayedStart()
    {
        DEBUG_WriteNewCrewTemplate();
        DEBUG_WriteNewRoomTemplate();

        DebugLog("Waiting 4 seconds to start delayed actions...");
        yield return new WaitForSeconds(4);
        DebugLog("4 seconds elapsed, running delayed actions.");
        isDoneLoading = true;
        //StartCoroutine("CreateFreshCrewMember", 2);
        StartCoroutine(FirebaseTimedUpdates(2.0f));

    }

    System.Collections.IEnumerator FirebaseTimedUpdates(float waitTimeSeconds)
    {
        DebugLog("Starting timed update sequence with interval " + waitTimeSeconds + " seconds.");
        while (true)
        {
            yield return new WaitForSeconds(waitTimeSeconds);
            DebugLog("[TimedUpdate] Triggered", DebugFlags.DatabaseOpsOnTimer);
            foreach (var item in toFirebasePush)
            {
                item.FirebaseUpdate(true); //run the update, marking it as a timed update
            }
        }
    }

    public void AddToFirebaseTimedUpdates(IFirebaseTimedUpdateable thingToAdd)
    {
        if (toFirebasePush.Contains(thingToAdd))
            DebugLog("[TimedUpdate] '" + thingToAdd + "' is already on the update list and wasn't added a second time.");
        else
        {
            DebugLog("[TimedUpdate] Adding '" + thingToAdd + "' to the timed update list.");
            toFirebasePush.Add(thingToAdd);
        }
    }

    public string Authenticate()
    {
        auth = gameObject.AddComponent<UserAuthentication>();
        Firebase.Auth.Credential token= auth.getCredential();
        DebugLog("Auth user token: " + token, DebugFlags.Auth);
        return "User1";
    }

    void LoadCrew()
    {
        Crew.BuildRandomNameList();
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + user_string + "/Crew/").GetValueAsync().ContinueWith(task =>
        {
           if (task.IsFaulted)
           {
               // Handle the error...
               DebugLog("Data retrival error when prompting for crew data!", DebugFlags.Critical);
           }
           else if (task.IsCompleted)
           {
               foreach(DataSnapshot crewMember in task.Result.Children)
               {
                   DebugLog("Found crewmember with ID " + crewMember.Key, DebugFlags.CrewLoadingOps);
                   SpawnCrew(crewMember.Key /*new List<object> { crewMember.Key, crewCreator }*/);
               }
           }
           else
           {
               //The task neither completed nor failed, this shouldn't happen. Should only be reached if task is canceled?
               DebugLog("Task error when prompting for crew data", DebugFlags.Critical);
           }
        });
    }

    private void SpawnCrew(string identifier)
    {
        DebugLog("Loading crew member " + identifier + " via dispatch...", DebugFlags.CrewLoadingOps);
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            //Debug.Log("This is executed from the main thread");
            Crew newCrewMember = Instantiate(crewCreator.prefab);

            newCrewMember.SendMessage("CrewCreatorStart", identifier);
            DebugLog("Loaded crew member with ID " + identifier, DebugFlags.CrewLoadingOps);

            CrewMembers.Add(newCrewMember);
            newCrewMember.transform.SetParent(crewCreator.transform);
        });

    }

    System.Collections.IEnumerator CreateFreshCrewMember(int count)
    {
        if (count > 0)
        {
            for (int index = 1; index <= count; index++)
            {
                DebugLog("Spawning fresh member (" + index + "/" + count + ") via dispatch...", DebugFlags.CrewLoadingOps);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Crew newCrewMember = Instantiate(crewCreator.prefab);

                    newCrewMember.SendMessage("FreshCrewSetup");
                    DebugLog("MainThread: Created a fresh crew member", DebugFlags.CrewLoadingOps);
                    CrewMembers.Add(newCrewMember);
                    newCrewMember.transform.SetParent(crewCreator.transform);
                });
                yield return new WaitForSecondsRealtime(1.0f); //TODO: Fix this bandaid. Because the spawn task is being enqueud in the main thread, this wait doesn't acutally wait for the purposes of the RNG, just enqueue time, which is why it has to be so long. Even then there might be overlap.
            }
            DebugLog("Done spawning requested " + count + " fresh crew members.");
        }
    }

    void DEBUG_WriteNewCrewTemplate() //This method overwrites the template in Firebase with the current fresh prefab's data
    {
        DebugLog("[DEBUG] Replacing new CREW MEMBER templace with crew prefab.");
        FirebaseDatabase.DefaultInstance.GetReference("new-object-templates/crew").SetRawJsonValueAsync(JsonUtility.ToJson(crewCreator.prefab));
    }

    void DEBUG_WriteNewRoomTemplate() //This method overwrites the template in Firebase with the current fresh prefab's data
    {
        DebugLog("[DEBUG] Replacing new ROOM templace with room prefab.");
        FirebaseDatabase.DefaultInstance.GetReference("new-object-templates/room").SetRawJsonValueAsync(JsonUtility.ToJson(roomCreator.prefab));
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

    private static void PrintEnabledDebugs()
    {
        Debug.Log("The following types of debug are enabled...");
        foreach (DebugFlags flag in (DebugFlags[]) System.Enum.GetValues(typeof(DebugFlags)))
        {
            DebugLog("" + flag.ToString(), flag);
        }
    }

    public static void DebugLog(string message, DebugFlags flagLevelToDisplayAt /*byte debugLevelToDisplayAt*/)
    {
        if ((debugLevelFlags & flagLevelToDisplayAt) != 0) //check if debugLevelFlags contains the flag passed in
        {
            if (flagLevelToDisplayAt == DebugFlags.Critical)
                Debug.LogError(message + "\n" + StackTraceUtility.ExtractStackTrace()); //TODO: make stack trace go to where this was called and not to this method
            else if(flagLevelToDisplayAt == DebugFlags.Warning)
                Debug.LogWarning(message + "\n" + StackTraceUtility.ExtractStackTrace());
            else
                Debug.Log(message + "\n (" + flagLevelToDisplayAt.ToString() + ")");

        }
    }

    /*public static void DebugLog(string message, byte debugLevelToDisplayAt) //alternate, old version of flags system
    {
        if (debugLevel >= debugLevelToDisplayAt)
            if(debugLevelToDisplayAt == 1)
                Debug.LogError(message);
            else
                Debug.Log(message);
    }*/

    public static void DebugLog(string message) //overloaded
    {
        DebugLog(message, DebugFlags.GeneralInfo);
    }
}
