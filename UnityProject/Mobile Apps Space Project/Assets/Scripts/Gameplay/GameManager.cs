﻿using Firebase.Database;
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
    SoundSystem = 1024,
    option13 = 2048,
    option14 = 4096
} //16 options with short

public class GameManager : MonoBehaviour
{
    public enum SceneSelected { title, login, main, option, map };
    public SceneSelected sceneCurrentlySelected = SceneSelected.main;
    public List<Crew> CrewMembers;
    public List<Room> Rooms;
    public Shared.Planets CurrentPlanetStringForDB = Shared.Planets.Loading;
    public GameObject startRoom;
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

    public const DebugFlags debugLevelFlags = /*DebugFlags.CrewLoadingOps |*/ DebugFlags.Critical | DebugFlags.Warning | DebugFlags.SoundSystem | /*DebugFlags.DatabaseOps | DebugFlags.DatabaseOpsOnTimer |*/ /*DebugFlags.Resources |*/ /*DebugFlags.CollisionOps |*/ DebugFlags.GeneralInfo | DebugFlags.ElapsedTime;
    //add or subtract values from DebugFlags to change what gets printed, or set to short.MaxValue to print everything
    //example debugLevelFlags = DebugFlags.Critical + DebugFlags.Warning + DebugFlags.CollisionOps

    public RuntimePlatform running_on;

    public static GameManager instance = null; //singleton pattern

    

    public ResourceManager resourceManager;
    public CrewSpawner crewCreator;// = UnityEngine.SceneManagement.SceneManager.GetSceneByName("01Gameplay").GetRootGameObjects().GetValue(0).;
    public RoomSpawner roomCreator;
    protected static UserAuthentication auth;
    private List<IFirebaseTimedUpdateable> toFirebasePush;
    private List<IProcessElapsedTime> toProcessElapsedTime;

    public static bool IsDoneLoading { get; private set; }
    public static bool IsNewUser = false;

    public string user_string = "StillLoading";

    public readonly static System.DateTime EpochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
    public int CurrentEpochTime { get; private set; }

    public AudioClip ButtonClickSoundEffect;
    private AudioSource ButtonClickSource;

    private void Start()
    {
        PrintEnabledDebugs();
        //DisplayLoadingScreen();

        toFirebasePush = new List<IFirebaseTimedUpdateable>();
        toProcessElapsedTime = new List<IProcessElapsedTime>();
        running_on = Application.platform;
        DebugLog("Running on a " + running_on, DebugFlags.GeneralInfo);
        resourceManager = gameObject.AddComponent(typeof(ResourceManager)) as ResourceManager;
        StartCoroutine(StartupCoroutine());
        //continues in coroutine
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

    System.Collections.IEnumerator StartupCoroutine()
    {
        //DEBUG_WriteNewCrewTemplate();
        //DEBUG_WriteNewRoomTemplate();
        //resourceManager.DEBUG_SetupResourcesList();

        Authenticate();

        yield return new WaitForSecondsRealtime(1);
        //give delay for room data to load in

        LoadCrew();

        //NewUserSetupAsync();
        DebugLog("Waiting to start delayed actions...");
        yield return new WaitForSecondsRealtime(2);
        DebugLog("... Time elapsed, running delayed actions.");

        StartCoroutine(FirebaseTimedUpdates(10.0f));
        yield return ProcessTimeSinceLastLogon();
        resourceManager.StartAveragesProcessing();

        IsDoneLoading = true;
        //HideLoadingScreen();
    }

    public void Authenticate()
    {
        string authToken = PlayerPrefs.GetString(Shared.PlayerPrefs_AuthTokenKey, "User1");
        Debug.Log("PlayerPrefs " + Shared.PlayerPrefs_AuthTokenKey + " contains '" + authToken + "'");
        if (authToken.Equals("")) //deal with empty auth tokens
            authToken = "User1";
        if (authToken.Equals("User1")) //If failed to load an auth token, use default user
            DebugLog("PlayerPrefs did not contain user auth token. Proceeding with User1 token instead.", DebugFlags.Warning);
        else
            DebugLog("Auth user token: " + authToken, DebugFlags.Auth);

        //authToken = "TestNewUser"; //uncomment this line to override for testing
        user_string = authToken;

        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + user_string + "/HasSetupRun").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                DebugLog("Data retrieval error when prompting for crew data!", DebugFlags.Critical);
            }
            else if (task.IsCompleted)
            {
                string data = task.Result.ToString(); //task.result.Value.toString() causes silent error when value is not present
                Debug.Log("HasSetupRun: " + data);
                if (!data.Equals("DataSnapshot { key = HasSetupRun, value = " + Shared.NewUser_HasSetupRunYesPhrase + " }")) //workaround for not present detection
                {
                    Debug.Log("Running new user setup");
                    UnityMainThreadDispatcher.Instance().Enqueue(() => 
                    {
                        StartCoroutine(CreateFreshCrewMember(Shared.NewUser_StartingCrewCount));
                    });
                    foreach (var thisRoom in Rooms)
                    {
                        thisRoom.NewUser_WriteMyRoomData();
                    }
                    resourceManager.NewUserSetupResourcesList();


                    FirebaseDatabase.DefaultInstance.GetReference("user-data/" + user_string + "/HasSetupRun").SetValueAsync(Shared.NewUser_HasSetupRunYesPhrase);
                }
                else
                {
                    Debug.Log("No need to run new user setup");
                }
            }
            else
            {
                //The task neither completed nor failed, this shouldn't happen. Should only be reached if task is canceled?
                DebugLog("Task error when prompting for crew data", DebugFlags.Critical);
            }
        });
    }

    System.Collections.IEnumerator FirebaseTimedUpdates(float waitTimeSeconds)
    {
        DebugLog("Starting timed update sequence with interval " + waitTimeSeconds + " seconds.");
        int counter = 0;
        while (true)
        {
            yield return new WaitForSeconds(waitTimeSeconds);
            counter++;
            DebugLog("[TimedUpdate] TimedUpdate #" + counter + " occurring", DebugFlags.DatabaseOpsOnTimer);
            
            FirebaseDatabase.DefaultInstance.GetReference("user-data/" + user_string + "/EpochTimeLastLogon").SetValueAsync(CurrentEpochTime);

            foreach (var item in toFirebasePush)
            {
                item.FirebaseUpdate(true); //run the update, marking it as a timed update
            }
            DebugLog("[TimedUpdate] TimedUpdate #" + counter + " done", DebugFlags.DatabaseOpsOnTimer);
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

    System.Collections.IEnumerator ProcessTimeSinceLastLogon()
    {
        DebugLog("Requesting time since last log-on...", DebugFlags.GeneralInfo);
        yield return FirebaseDatabase.DefaultInstance.GetReference("user-data/" + user_string + "/EpochTimeLastLogon").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                DebugLog("Data retrieval error when prompting for EpochTimeLastLogon!", DebugFlags.Critical);
            }
            else if (task.IsCompleted)
            {
                System.Int64 lastTime = (System.Int64)task.Result.Value;
                var deltaTime = (CurrentEpochTime - lastTime);
                //Debug.Log(task.Result.Value.GetType());
                DebugLog("Last log-on time: " + lastTime + "\tTime difference: " + deltaTime, DebugFlags.ElapsedTime);
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

    void LoadCrew()
    {
        Crew.BuildRandomNameList();
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + user_string + "/Crew/").GetValueAsync().ContinueWith(task =>
        {
           if (task.IsFaulted)
           {
               // Handle the error...
               DebugLog("Data retrieval error when prompting for crew data!", DebugFlags.Critical);
           }
           else if (task.IsCompleted)
           {
               foreach(DataSnapshot crewMember in task.Result.Children)
               {
                   DebugLog("Retrieved crew member with ID " + crewMember.Key, DebugFlags.CrewLoadingOps);
                   SpawnCrew(crewMember.Key);
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
            Crew newCrewMember = Instantiate(crewCreator.prefab);

            //newCrewMember.SendMessage("CrewCreatorStart", identifier);
            newCrewMember.StartCoroutine(newCrewMember.CrewCreatorStartMultithread(identifier));
            //DebugLog("Loaded crew member with ID " + identifier, DebugFlags.CrewLoadingOps);

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
                    //newCrewMember.SendMessage("FreshCrewSetup");
                    newCrewMember.StartCoroutine(newCrewMember.SetUpAndWriteFreshCrew());
                    DebugLog("MainThread: Created a fresh crew member", DebugFlags.CrewLoadingOps);
                    CrewMembers.Add(newCrewMember);
                    newCrewMember.transform.SetParent(crewCreator.transform);
                });
                yield return new WaitForSecondsRealtime(1.0f); //TODO: Fix this band-aid. Because the spawn task is being enqueued in the main thread, this wait doesn't actually wait for the purposes of the RNG, just enqueue time, which is why it has to be so long. Even then there might be overlap.
            }
            DebugLog("Done spawning requested " + count + " fresh crew members.");
        }
    }

    void DEBUG_WriteNewCrewTemplate() //This method overwrites the template in Firebase with the current fresh prefab's data
    {
        DebugLog("[DEBUG] Replacing new CREW MEMBER template with crew prefab.");
        FirebaseDatabase.DefaultInstance.GetReference("new-object-templates/crew").SetRawJsonValueAsync(JsonUtility.ToJson(crewCreator.prefab.AllData));
    }

    void DEBUG_WriteNewRoomTemplate() //This method overwrites the template in Firebase with the current fresh prefab's data
    {
        DebugLog("[DEBUG] Replacing new ROOM template with room prefab.");
        FirebaseDatabase.DefaultInstance.GetReference("new-object-templates/room").SetRawJsonValueAsync(JsonUtility.ToJson(roomCreator.prefab.data));
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
            if(flag != DebugFlags.Warning && flag != DebugFlags.Critical)
                DebugLog("" + flag.ToString(), flag);
            else
                DebugLog("" + flag.ToString());
        }
        Debug.Log("=======================================");
    }

    public void PlayButtonSound()
    {
        if(ButtonClickSource == null)
        {
            ButtonClickSource = gameObject.AddComponent<AudioSource>();//.playOnAwake = false;
            ButtonClickSource.playOnAwake = false;
            if (ButtonClickSoundEffect == null)
            {
                DebugLog("No button click sound effect set! Set it in the inspector.", DebugFlags.Warning);
            }
            ButtonClickSource.clip = ButtonClickSoundEffect;
            DebugLog("Added an audio souce to " + name, DebugFlags.SoundSystem);
        }
        ButtonClickSource.Play();
        DebugLog("Attempted to play button sound.", DebugFlags.SoundSystem);
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

    public void CreateNewCrew()
    {
        StartCoroutine(CreateFreshCrewMember(1));

    }

    public void ChangeCurrentScene(int newSceneNum)
    {
        sceneCurrentlySelected = (SceneSelected)newSceneNum;
    }
}
