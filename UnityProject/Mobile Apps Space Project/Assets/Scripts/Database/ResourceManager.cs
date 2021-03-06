﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

using UnityEngine;

public class ResourceManager : MonoBehaviour, IFirebaseTimedUpdateable, IProcessElapsedTime {

    //public Dictionary<Shared.ResourceTypes, int> resources;
    public DictionaryOfResourceAndFloat resources;

    public DictionaryOfResourceAndFloat resourcesAverages;

    public void Start()
    {
        GameManager.DebugLog("Starting resource manager...");
        resources = new DictionaryOfResourceAndFloat();
        resourcesAverages = new DictionaryOfResourceAndFloat();
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Resources").ValueChanged += HandleValueChanged;
        GameManager.instance.AddToFirebaseTimedUpdates(this);
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote resources with JSON from database: " + json, DebugFlags.DatabaseOps);
        JsonUtility.FromJsonOverwrite(json, resources);
    }

    public float GetResource(Shared.ResourceTypes resourceToGet, bool DONT_CALL_THIS_FROM_ANYWHERE_BUT_GAMEMANAGER_OR_RESOURCEMANAGER)
    {
        if (resources.ContainsKey(resourceToGet))
            return resources[resourceToGet];
        else
        {
            GameManager.DebugLog("A resource " + resourceToGet + " was requested that was not present in the resources list; returning 0 count.", DebugFlags.Warning);
            return 0;
        }
    }

    public float SetResource(Shared.ResourceTypes key, float value, bool DONT_CALL_THIS_FROM_ANYWHERE_BUT_GAMEMANAGER_OR_RESOURCEMANAGER)
    {
        GameManager.DebugLog("The resource " + key.ToString() + " was SET to " + value, DebugFlags.Resources);
        resources[key] = value;
        return value;
    }

    public float ChangeResource(Shared.ResourceTypes key, float deltaValue, bool DONT_CALL_THIS_FROM_ANYWHERE_BUT_GAMEMANAGER_OR_RESOURCEMANAGER)
    {
        if (resources.ContainsKey(key))
        {
            GameManager.DebugLog("The resource " + key.ToString() + " was CHANGED by " + deltaValue, DebugFlags.Resources);
            resources[key] += deltaValue;
            return resources[key];
        }
        else
        {
            GameManager.DebugLog("A resource " + key + " was CHANGE requested that was not present in the resources list; returning 0 count.", DebugFlags.Warning);
            return 0;
        }
        
    }

    public void NewUserSetupResourcesList()
    {
        GameManager.DebugLog("[NewUser] Creating new user resources and writing to database...");

        resources.Add(Shared.ResourceTypes.minerals, 100.0f);
        resources.Add(Shared.ResourceTypes.food, 100.0f);
        resources.Add(Shared.ResourceTypes.water, 100.0f);
        resources.Add(Shared.ResourceTypes.money, 100.0f);
        resources.Add(Shared.ResourceTypes.energy, 100.0f);
        resources.Add(Shared.ResourceTypes.preciousMetal, 100.0f);
        resources.Add(Shared.ResourceTypes.premiumCurrency, -1.0f);

        GameManager.DebugLog("[NewUser] Resources count after DEBUG setup: " + resources.Count);
        GameManager.DebugLog("[NewUser] Resources JSON to write: " + JsonUtility.ToJson(resources));

        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Resources").SetRawJsonValueAsync(JsonUtility.ToJson(resources));

    }

    public void StartAveragesProcessing()
    {
        GameManager.DebugLog("Starting averages coroutine...", DebugFlags.DatabaseOpsOnTimer);
        StartCoroutine(CalculateAverages(10.0f));
    }

    public IEnumerator CalculateAverages(float intervalSeconds)
    {
        int counter = 0;
        while (true)
        {
            counter++;
            GameManager.DebugLog("(Loop #" + counter + ") Calculating resource averages over " + intervalSeconds + " seconds...", DebugFlags.DatabaseOpsOnTimer);
            DictionaryOfResourceAndFloat pastResources = new DictionaryOfResourceAndFloat();

            foreach (var item in resources)
            {
                pastResources.Add(item.Key, item.Value);
            }

            yield return new WaitForSeconds(intervalSeconds);

            resourcesAverages.Clear();
            foreach (var item in resources)
            {
                float average = (item.Value - pastResources[item.Key]) / intervalSeconds;
                //GameManager.DebugLog("Resource " + item.Key + " average calculated to be " + average, DebugFlags.DatabaseOpsOnTimer);
                resourcesAverages.Add(item.Key, average);
            }

            string json2 = JsonUtility.ToJson(resourcesAverages);
            FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/ResourceAverages").SetRawJsonValueAsync(json2);
            GameManager.DebugLog("(Loop #" + counter + ") Updated user's resources averages database contents with " + json2 /*+ "\n Waiting " + intervalSeconds + " seconds for next round."*/, DebugFlags.DatabaseOpsOnTimer);
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void FirebaseUpdate(bool wasTimedUpdate)
    {
        string json = JsonUtility.ToJson(resources);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Resources").SetRawJsonValueAsync(json);
        if (wasTimedUpdate)
            GameManager.DebugLog("[TimedUpdate] Updated user's resources database contents with " + json, DebugFlags.DatabaseOpsOnTimer);
        else
            GameManager.DebugLog("[>TriggeredUpdate] Updated user's resources database contents with " + json, DebugFlags.DatabaseOps);
    }

    public void ProcessTime(float deltaTime)
    {
        GameManager.DebugLog("Waiting for resource averages to process elapsed time...", DebugFlags.ElapsedTime);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/EpochTimeLastLogon").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                GameManager.DebugLog("Data retrieval error when prompting for Resource Averages!", DebugFlags.Critical);
            }
            else if (task.IsCompleted)
            {
                string json = (string)task.Result.Value;
                GameManager.DebugLog("...Loaded resources averages: " + json, DebugFlags.ElapsedTime);
                JsonUtility.FromJsonOverwrite(json, resourcesAverages);
                foreach (var item in resourcesAverages)
                {
                    float deltaValue = item.Value * deltaTime;
                    GameManager.DebugLog("Changed " + item.Key + " by " + deltaValue, DebugFlags.ElapsedTime);
                    ChangeResource(item.Key, deltaValue, true);
                }
            }
            else
            {
                //The task neither completed nor failed, this shouldn't happen. Should only be reached if task is canceled?
                GameManager.DebugLog("Task error when prompting for Resource Averages", DebugFlags.Critical);
            }
        });
        
    }
}

