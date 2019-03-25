﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

using UnityEngine;

public class ResourceManager {

    //public Dictionary<Shared.ResourceTypes, int> resources;
    public DictionaryOfResourceAndFloat resources;

    public ResourceManager()
    {
        GameManager.DebugLog("Starting resource manager...");
        resources = new DictionaryOfResourceAndFloat();
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Resources").ValueChanged += HandleValueChanged;
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote resources with JSON from database: " + json, 4);
        //JsonUtility.FromJsonOverwrite(json, resources);
    }

    public float GetResource(Shared.ResourceTypes resourceToGet)
    {
        if (resources.ContainsKey(resourceToGet))
            return resources[resourceToGet];
        else
        {
            GameManager.DebugLog("A resource " + resourceToGet + " was requested that was not present in the resources list; returning 0 count.", 2);
            return 0;
        }
    }

    public float SetResource(Shared.ResourceTypes key, float value)
    {
        resources[key] = value;
        return value;
    }

    public float ChangeResource(Shared.ResourceTypes key, float deltaValue)
    {
        resources[key] += deltaValue;
        return resources[key];
    }

    public void DEBUG_SetupResourcesList()
    {
        GameManager.DebugLog("DEBUG: Creating demo resouces and writing to database...");

        resources.Add(Shared.ResourceTypes.scraps, 5.4f);
        resources.Add(Shared.ResourceTypes.money, 10.2f);
        resources.Add(Shared.ResourceTypes.energy, 7.1f);

        GameManager.DebugLog("Resources count after DEBUG setup: " + resources.Count);
        GameManager.DebugLog("Resources JSON to write: " + JsonUtility.ToJson(resources));

        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Resources").SetRawJsonValueAsync(JsonUtility.ToJson(resources));

    }
}

