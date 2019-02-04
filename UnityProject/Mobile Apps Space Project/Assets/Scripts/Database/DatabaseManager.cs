using System.Collections;
using System.Collections.Generic;

using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

using UnityEngine;

public class DatabaseManager {

    public static int maximumTimeout = 10000;

    //written with the guidance of https://firebase.google.com/docs/database/unity/retrieve-data

    private FirebaseDatabase instance;
    //private DatabaseReference rootRef; 

        /*
    //Start Singleton handling from https://gamedev.stackexchange.com/a/116010
    private static DatabaseManager _instance;
    public static DatabaseManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            //Destroy(this.gameObject);
            Debug.LogError("A second database manager was created! Unexpected behaviour will ensue.");
        }
        else
        {
            _instance = this;
        }
    }
    //End singleton handling from https://gamedev.stackexchange.com/a/116010
        */
    
    // Use this for initialization
    public DatabaseManager() {
        // Set this before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://mobileappsmrhp-spacecolony.firebaseio.com/");

        instance = FirebaseDatabase.DefaultInstance;
        //rootRef = instance.RootReference;

        GameManager.DebugLog("Database Manager initialized", 3);
    }
	
    /*
	// Update is called once per frame
	void Update () {
		
	}
    */

    private IEnumerator Test()
    {

    }

    //Run this to get a datavase value
    public object GetValueOnce(string reference)
    {
        //Since the database ref could be storing any number of things, you'll need to use System.Convert to convert it to the needed type, I think. - Rob
        // https://docs.microsoft.com/en-us/dotnet/api/system.convert?view=netframework-4.7.2

        object output = null;
        bool busy = true;
        int timeBusy = 0;

        GameManager.DebugLog("Handling request for data at " + reference + "...", 4);

        instance.GetReference(reference).GetValueAsync()
            .ContinueWith(task => {
              if (task.IsFaulted)
              {
                // Handle the error...
                GameManager.DebugLog("Data retrival error when prompting for data at reference: " + reference + ", returning null instead.", 1);
              }
              else if (task.IsCompleted)
              {
                output = task.Result.GetValue(false);
                    Debug.Log("got value: " + output);
              }
              else
              {
                //The task neither completed nor failed, this shouldn't happen. Should only be reached if task is canceled?
                GameManager.DebugLog("Task error when prompting for data at reference: " + reference, 1);
              }
              busy = false;
            });

        while (busy && timeBusy < maximumTimeout) //wait until maximum cutoff reached 
        {
            timeBusy++;
            
        }
        if (timeBusy >= maximumTimeout)
            GameManager.DebugLog("DatabaseManager exceeded maximum timeout while handling request for " + reference, 1);
        else
            GameManager.DebugLog("Requested for " + reference + " completed after " + timeBusy + " time counts with the value " + output, 4);
        return output;
    }

    //
    public object GetValueListener(string reference)
    {
        //TODO: Make this based off of the example on the website
        return null;
    }

    //TODO: return error codes needed?
    public void SetValueAsync(string reference, object thing)
    {
        GameManager.DebugLog("Overwriting data at " + reference + " with " + thing.ToString(), 4);
        instance.GetReference(reference).SetValueAsync(thing);
    }

    //TODO: return error codes needed?
    public void SetJsonAsyc(string reference, object thing)
    {
        string json = JsonUtility.ToJson(thing);
        GameManager.DebugLog("Overwriting JSON at: " + reference + " with: " + json, 4);
        instance.GetReference(reference).SetRawJsonValueAsync(json);
    }


}
