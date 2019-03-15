﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

using UnityEngine;

public class ResourceManager {


    public Dictionary<Shared.ResourceTypes, int> resources;

    public ResourceManager()
    {
        GameManager.DebugLog("Starting resource manager...");
        /*FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Resources").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                GameManager.DebugLog("Data retrival error when prompting for resources!", 1);
            }
            else if (task.IsCompleted)
            {
                JsonUtility.FromJsonOverwrite(task.Result.GetRawJsonValue(), this);
                GameManager.DebugLog("Overwrote resources with data from database: " + task.Result.GetRawJsonValue(), 3);
            }
            else
            {
                //The task neither completed nor failed, this shouldn't happen. Should only be reached if task is canceled?
                GameManager.DebugLog("Task error when prompting for resources", 1);
            }
        });*/
        resources = new Dictionary<Shared.ResourceTypes, int>();
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Resources").ValueChanged += HandleValueChanged;
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote ResouceManager with JSON from database: " + json, 4);
        JsonUtility.FromJsonOverwrite(json, this);
    }

    int GetResource(Shared.ResourceTypes resourceToGet)
    {
        if (resources.ContainsKey(resourceToGet))
            return resources[resourceToGet];
        else
        {
            GameManager.DebugLog("A resource " + resourceToGet + " was requested that was not present in the resources list.", 2);
            return 0;
        }
    }

    public int SetResource(Shared.ResourceTypes key, int value)
    {
        resources[key] = value;
        return value;
    }

    public int ChangeResource(Shared.ResourceTypes key, int deltaValue)
    {
        resources[key] += deltaValue;
        return resources[key];
    }

    public void DEBUG_SetupResourcesList()
    {
        GameManager.DebugLog("DEBUG: Creating demo resouces and writing to database...");
        /*resources = new Dictionary<Shared.ResourceTypes, int>()
        {
            {Shared.ResourceTypes.scraps, 5 },
            {Shared.ResourceTypes.money, 10 },
            {Shared.ResourceTypes.energy, 7 }
        };*/

        resources.Add(Shared.ResourceTypes.scraps, 5);
        resources.Add(Shared.ResourceTypes.money, 10);
        resources.Add(Shared.ResourceTypes.energy, 7);

        GameManager.DebugLog("Resources count after DEBUG setup: " + resources.Count);

        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Resources").SetRawJsonValueAsync(JsonUtility.ToJson(this));

    }
}

