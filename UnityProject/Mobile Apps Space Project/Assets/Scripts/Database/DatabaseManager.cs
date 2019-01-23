using System.Collections;
using System.Collections.Generic;

using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

using UnityEngine;

public class DatabaseManager : MonoBehaviour {

    //written with the guidance of https://firebase.google.com/docs/database/unity/retrieve-data

    private FirebaseDatabase instance;
    //private DatabaseReference rootRef; 


    //Start Singleton handling from https://gamedev.stackexchange.com/a/116010
    private static DatabaseManager _instance;
    public static DatabaseManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    //End singleton handling from https://gamedev.stackexchange.com/a/116010

    
    // Use this for initialization
    void Start () {
        // Set this before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://mobileappsmrhp-spacecolony.firebaseio.com/");

        instance = FirebaseDatabase.DefaultInstance;
        //rootRef = instance.RootReference;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Run this to get a datavase value
    public object GetValueOnce(string reference)
    {
        //Since the database ref could be storing any number of things, you'll need to use System.Convert to convert it to the needed type, I think. - Rob
        // https://docs.microsoft.com/en-us/dotnet/api/system.convert?view=netframework-4.7.2

        object output = null;

        instance.GetReference(reference).GetValueAsync()
            .ContinueWith(task => {
              if (task.IsFaulted)
                {
                    // Handle the error...
                    if (GameManager.debugLevel >= 1)
                        Debug.LogError("Data retrival error when prompting for data at reference: " + reference + ", returning null instead.");
                }
              else if (task.IsCompleted)
                {
                    output = task.Result.GetValue(false);
                }
              else
                {
                    //The task neither completed nor failed, this shouldn't happen. Should only be reached if task is canceled?
                    if(GameManager.debugLevel >= 1)
                        Debug.LogError("Task error when prompting for data at reference: " + reference);
                }
          });

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
        if (GameManager.debugLevel >= 4)
        {
            Debug.Log("Overwriting data at " + reference + " with " + thing.ToString());
        }
        instance.GetReference(reference).SetValueAsync(thing);
    }

    //TODO: return error codes needed?
    public void SetJsonAsyc(string reference, object thing)
    {
        string json = JsonUtility.ToJson(thing);
        if (GameManager.debugLevel >= 4)
        {
            Debug.Log("Overwriting JSON at: " + reference + " with: " + json);
        }
        instance.GetReference(reference).SetRawJsonValueAsync(json);
    }
}
