using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

using UnityEngine;

public class DatabaseManager {

    //public static int maximumTimeout = 10000;

    //written with the guidance of https://firebase.google.com/docs/database/unity/retrieve-data

    private FirebaseDatabase instance;
    private string user_string;

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
    public DatabaseManager(string UserID) {
        // Set this before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://mobileappsmrhp-spacecolony.firebaseio.com/");

        user_string = UserID;

        instance = FirebaseDatabase.DefaultInstance;
        //rootRef = instance.RootReference;

        GameManager.DebugLog("Database Manager initialized", 3);
    }

    /*
	// Update is called once per frame
	void Update () {
		
	}
    */

    /*private class DatabaseGetUtility
    {
        public bool IsBusy { get; private set; }
        public object returnValue { get; private set; }

        public IEnumerator LoadDataAsync()
        {
            IsBusy = true;
            yield return new WaitForSeconds(2);
            IsBusy = false;
        }

        async Task LoadDataAsyncAwait(string reference)
        {
            object output = null;

            Task.

            Task afterRequest => {
                if (afterRequest.IsFaulted)
                {
                    // Handle the error...
                    GameManager.DebugLog("Data retrival error when prompting for data at reference: " + reference + ", returning null instead.", 1);
                }
                else if (afterRequest.IsCompleted)
                {
                    output = afterRequest.Result.GetValue(false);
                    Debug.Log("got value: " + output);
                }
                else
                {
                    //The task neither completed nor failed, this shouldn't happen. Should only be reached if task is canceled?
                    GameManager.DebugLog("Task error when prompting for data at reference: " + reference, 1);
                }
            }

            await instance.GetReference(reference).GetValueAsync()
            .ContinueWith(afterRequest);
            return 
        }
    }*/

    //Run this to get a datavase value
    /*public object GetValueOnce(string reference)
    {
        //Since the database ref could be storing any number of things, you'll need to use System.Convert to convert it to the needed type, I think. - Rob
        // https://docs.microsoft.com/en-us/dotnet/api/system.convert?view=netframework-4.7.2

        object output = null;
        bool busy = true;
        int timeBusy = 0;

        GameManager.DebugLog("Handling request for data at " + reference + "...", 4);

        //DatabaseGetUtility utility = new DatabaseGetUtility();

        //utility.LoadDataAsyncAwait(reference);

        //var result = AsyncContext

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
    }*/

    public void LoadCrew()
    {
        
    }

    public void LoadRooms()
    {

    }

    public void LoadResources()
    {

    }

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

