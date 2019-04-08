using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

using UnityEngine;

public class ResourceManager : IFirebaseTimedUpdateable, IProcessElapsedTime {

    //public Dictionary<Shared.ResourceTypes, int> resources;
    public DictionaryOfResourceAndFloat resources;

    public DictionaryOfResourceAndFloat resourcesAverages;

    public ResourceManager()
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
        GameManager.DebugLog("The resource " + key.ToString() + " was CHANGED by " + deltaValue, DebugFlags.Resources);
        resources[key] += deltaValue;
        return resources[key];
    }

    public void DEBUG_SetupResourcesList()
    {
        GameManager.DebugLog("[DEBUG] Creating demo resouces and writing to database...");

        resources.Add(Shared.ResourceTypes.minerals, 5.4f);
        resources.Add(Shared.ResourceTypes.food, 25.5f);
        resources.Add(Shared.ResourceTypes.water, 6.3f);
        resources.Add(Shared.ResourceTypes.money, 10.7f);
        resources.Add(Shared.ResourceTypes.energy, 7.1f);
        resources.Add(Shared.ResourceTypes.preciousMetal, 4.2f);
        resources.Add(Shared.ResourceTypes.premiumCurrency, 999.99f);

        GameManager.DebugLog("[DEBUG] Resources count after DEBUG setup: " + resources.Count);
        GameManager.DebugLog("[DEBUG] Resources JSON to write: " + JsonUtility.ToJson(resources));

        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Resources").SetRawJsonValueAsync(JsonUtility.ToJson(resources));

    }

    private IEnumerator CalculateAverages(float intervalSeconds)
    {
        GameManager.DebugLog("Calculating resource averages over " + intervalSeconds + " seconds...", DebugFlags.DatabaseOpsOnTimer);
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
            GameManager.DebugLog("Resource " + item.Key + " average calculated to be " + average, DebugFlags.DatabaseOpsOnTimer);
            resourcesAverages.Add(item.Key, average);
        }

        string json2 = JsonUtility.ToJson(resourcesAverages);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/ResourceAverages").SetRawJsonValueAsync(json2);
        GameManager.DebugLog("Updated user's resources averages database contents with " + json2, DebugFlags.DatabaseOpsOnTimer);
        yield return new WaitForSeconds(intervalSeconds);
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
                GameManager.DebugLog("Data retrival error when prompting for Resource Averages!", DebugFlags.Critical);
            }
            else if (task.IsCompleted)
            {
                string json = (string)task.Result.Value;
                GameManager.DebugLog("...Loaded resources averages: " + json, DebugFlags.ElapsedTime);
                JsonUtility.FromJsonOverwrite(json, resourcesAverages);
                foreach (var item in resourcesAverages)
                {
                    GameManager.DebugLog("Changed " + item.Key + " by " + item.Value, DebugFlags.ElapsedTime);
                    ChangeResource(item.Key, FIXTHISLATER, true);
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

