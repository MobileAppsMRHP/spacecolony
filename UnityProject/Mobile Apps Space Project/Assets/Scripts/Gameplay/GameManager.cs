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
    ElapsedTime = 512,
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

    public const DebugFlags debugLevelFlags = DebugFlags.Critical | DebugFlags.Warning | DebugFlags.DatabaseOps | DebugFlags.Resources | DebugFlags.CollisionOps | DebugFlags.GeneralInfo | DebugFlags.ElapsedTime;
    //add or subtract values from DebugFlags to change what gets printed, or set to short.MaxValue to print everything
    //example debugLevelFlags = DebugFlags.Critical + DebugFlags.Warning + DebugFlags.CollisionOps

    public RuntimePlatform running_on;

    public static GameManager instance = null; //singleton pattern

    public ResourceManager resourceManager;
    public CrewSpawner crewCreator;
    public RoomSpawner roomCreator;
    protected static UserAuthentication auth;
    private List<IFirebaseTimedUpdateable> toFirebasePush;
    private List<IProcessElapsedTime> toProcessElapsedTime;

    public static bool IsDoneLoading { get; private set; }

    public string user_string = "StillLoading";

    public readonly static System.DateTime EpochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
    public int CurrentEpochTime { get; private set; }

    private void Start()
    {
        PrintEnabledDebugs();
        StartCoroutine("DebugDelayedStart");

        DisplayLoadingScreen();

        toFirebasePush = new List<IFirebaseTimedUpdateable>();
        toProcessElapsedTime = new List<IProcessElapsedTime>();
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
        CurrentEpochTime = (int)(System.DateTime.UtcNow - EpochStart).TotalSeconds;
    }

    System.Collections.IEnumerator DebugDelayedStart()
    {
        DEBUG_WriteNewCrewTemplate();
        DEBUG_WriteNewRoomTemplate();

        DebugLog("Waiting 4 seconds to start delayed actions...");
        yield return new WaitForSeconds(4);
        DebugLog("4 seconds elapsed, running delayed actions.");
        IsDoneLoading = true;
        //StartCoroutine("CreateFreshCrewMember", 2);
        StartCoroutine(FirebaseTimedUpdates(10.0f));
        ProcessTimeSinceLastLogon();

    }

    System.Collections.IEnumerator FirebaseTimedUpdates(float waitTimeSeconds)
    {
        DebugLog("Starting timed update sequence with interval " + waitTimeSeconds + " seconds.");
        while (true)
        {
            yield return new WaitForSeconds(waitTimeSeconds);
            DebugLog("[TimedUpdate] Triggered", DebugFlags.DatabaseOpsOnTimer);
            
            
            FirebaseDatabase.DefaultInstance.GetReference("user-data/" + user_string + "/EpochTimeLastLogon").SetValueAsync(CurrentEpochTime);
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

    private void ProcessTimeSinceLastLogon()
    {
        DebugLog("Requesting time since last logon...", DebugFlags.GeneralInfo);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + user_string + "/EpochTimeLastLogon").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                DebugLog("Data retrival error when prompting for EpochTimeLastLogon!", DebugFlags.Critical);
            }
            else if (task.IsCompleted)
            {
                System.Int64 lastTime = (System.Int64)task.Result.Value;
                var deltaTime = (CurrentEpochTime - lastTime);
                //Debug.Log(task.Result.Value.GetType());
                DebugLog("Last logon time: " + lastTime + "\tTime difference: " + deltaTime, DebugFlags.ElapsedTime);
                foreach (var item in toProcessElapsedTime)
                {
                    item.ProcessTime(deltaTime);
                }
            }
            else
            {
                //The task neither completed nor failed, this shouldn't happen. Should only be reached if task is canceled?
                DebugLog("Task error when prompting for EpochTimeLastLogon", DebugFlags.Critical);
            }
        });
    }

    public void AddToProcessElapsedTime(IProcessElapsedTime thingToAdd)
    {
        if (toProcessElapsedTime.Contains(thingToAdd))
            DebugLog("[ProcessElapsed] '" + thingToAdd + "' is already on the process elapsed list and wasn't added a second time.", DebugFlags.ElapsedTime);
        else
        {
            DebugLog("[ProcessElapsed] Adding '" + thingToAdd + "' to the process elapsed list.", DebugFlags.ElapsedTime);
            toProcessElapsedTime.Add(thingToAdd);
        }
    }

    public string Authenticate()
    {
        /*auth = gameObject.AddComponent<UserAuthentication>();
        Firebase.Auth.Credential token= auth.getCredential();
        auth.DisableUI();
        auth.DisableUI();*/
        string authToken = PlayerPrefs.GetString("UserAuthToken", "User1");
        if (authToken.Equals("User1"))
            DebugLog("PlayerPrefs did not contain user auth token. Proceeding with User1 token instead.", DebugFlags.Warning);
        else
            DebugLog("Auth user token: " + authToken, DebugFlags.Auth);
        return authToken;
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

    public float GetResource(Shared.ResourceTypes resourceToGet)
    {
        if (resourceManager == null)
        {
            DebugLog("A resource " + resourceToGet + " was requested, but ResourceManager does not yet exist; returning 0 count.", DebugFlags.Warning);
            return 0;
        }
        return resourceManager.GetResource(resourceToGet, true);
    }

    public float SetResource(Shared.ResourceTypes resourceToGet, float value)
    {
        if (resourceManager == null)
        {
            DebugLog("A resource " + resourceToGet + " could not be set because ResourceManager does not yet exist; returning 0 count and no change.", DebugFlags.Warning);
            return 0;
        }
        return resourceManager.SetResource(resourceToGet, value, true);
    }

    public float ChangeResource(Shared.ResourceTypes resourceToGet, float deltaValue)
    {
        if (resourceManager == null)
        {
            DebugLog("A resource " + resourceToGet + " could not be changed because ResourceManager does not yet exist; returning 0 count and no change.", DebugFlags.Warning);
            return 0;
        }
        return resourceManager.ChangeResource(resourceToGet, deltaValue, true);
    }

    private static void PrintEnabledDebugs()
    {
        Debug.Log("=======================================\nThe following types of debug are enabled...");
        foreach (DebugFlags flag in (DebugFlags[]) System.Enum.GetValues(typeof(DebugFlags)))
        {
            DebugLog("" + flag.ToString(), flag);
        }
        Debug.Log("=======================================");
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
