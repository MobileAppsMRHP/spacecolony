using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

using UnityEngine;

public class ResourceManager : IFirebaseTimedUpdateable {

    //public Dictionary<Shared.ResourceTypes, int> resources;
    public DictionaryOfResourceAndFloat resources;

    public ResourceManager()
    {
        GameManager.DebugLog("Starting resource manager...");
        resources = new DictionaryOfResourceAndFloat();
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Resources").ValueChanged += HandleValueChanged;
        GameManager.instance.AddToFirebaseTimedUpdates(this);
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote resources with JSON from database: " + json, DebugFlags.DatabaseOps);
        JsonUtility.FromJsonOverwrite(json, resources);
    }

    public float GetResource(Shared.ResourceTypes resourceToGet)
    {
        if (resources.ContainsKey(resourceToGet))
            return resources[resourceToGet];
        else
        {
            GameManager.DebugLog("A resource " + resourceToGet + " was requested that was not present in the resources list; returning 0 count.", DebugFlags.Warning);
            return 0;
        }
    }

    public float SetResource(Shared.ResourceTypes key, float value)
    {
        GameManager.DebugLog("The resource " + key.ToString() + " was SET to " + value, DebugFlags.Resources);
        resources[key] = value;
        return value;
    }

    public float ChangeResource(Shared.ResourceTypes key, float deltaValue)
    {
        GameManager.DebugLog("The resource " + key.ToString() + " was CHANGED by " + deltaValue, DebugFlags.Resources);
        resources[key] += deltaValue;
        return resources[key];
    }

    public void DEBUG_SetupResourcesList()
    {
        GameManager.DebugLog("[DEBUG] Creating demo resouces and writing to database...");

        /*
        minerals,
        food,
        water,
        money,
        energy,
        preciousMetal,
        premiumCurrency
        */

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

    public void FirebaseUpdate(bool wasTimedUpdate)
    {
        string json = JsonUtility.ToJson(resources);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Resources").SetRawJsonValueAsync(json);
        if (wasTimedUpdate)
            GameManager.DebugLog("[TimedUpdate] Updated user's resources database contents with " + json, DebugFlags.DatabaseOpsOnTimer);
        else
            GameManager.DebugLog("[>TriggeredUpdate] Updated user's resources database contents with " + json, DebugFlags.DatabaseOps);
    }
}

