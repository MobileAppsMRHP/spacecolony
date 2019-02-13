using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.DefaultControls;

public class GameManager : MonoBehaviour
{

    private List<Crew> CrewMembers { get; set; }
    public Dictionary<string, int> Resources; // { get; set; }

    /*public float food;
    public float energy;
    public float water;
    public float money;*/

    private bool IsLoading = false; //indicates to other things if the game is loading

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

    public RuntimePlatform running_on;

    public static GameManager instance = null; //singleton pattern

    private static DatabaseManager dbman = null;

    private void Start()
    {
        DisplayLoadingScreen();
        running_on = Application.platform;
        DebugLog("Running on a " + running_on, 3);
        SetupResourcesList();
        LoadDatabaseValues();
        HideLoadingScreen();
    }

    // Use this for initialization
    void Awake () { //Only initializes once
		if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetupResourcesList()
    {
        Resources.Add("food", 0);
        Resources.Add("water", 0);
        Resources.Add("energy", 0);
        Resources.Add("money", 0);
        DebugLog("Done setting up resrouces list",3);
    }

    /*public void AddFood(int num)
    {
        food += num;
    }*/

    //TODO: make this actually load database values, initalize crew and resources, etc.
    public void LoadDatabaseValues()
    {
        dbman = new DatabaseManager("User1"); //TODO: get actual user uuid from auth

        dbman.LoadCrew();
        dbman.LoadRooms();
        dbman.LoadResources();
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

    public static void DebugLog(string message, byte debugLevelToDisplayAt)
    {
        if (debugLevel >= debugLevelToDisplayAt)
            if(debugLevelToDisplayAt == 1)
                Debug.LogError(message);
            else
                Debug.Log(message);
    }

    public static void DebugLog(string message) //overloaded
    {
        DebugLog(message, 0);
    }
}
