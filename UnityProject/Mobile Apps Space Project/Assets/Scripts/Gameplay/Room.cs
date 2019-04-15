using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Room : MonoBehaviour, IFirebaseTimedUpdateable {

    public int peopleLimit;
    public int roomLevel;
    public List<Crew> crewInThisRoom;
    public List<GameObject> crewLocations;
    List<Vector3> UpgradeResourceMultiplier; //mineral (scraps), energy, money
    public Vector3 upgradeCosts;
    public Shared.RoomTypes RoomType;
    public bool currentlySelected;
    private GameManager gameManager;
    public GameObject mainScreen;
    List<CrewSkills> crewSkillsResourceMultipliers;

    //public string RoomUniqueIdentifierForDB;
    Vector3 baseUpgradeCost = new Vector3(50, 50, 50);
    struct CrewSkills
    {
        public float fighting;
        public float medical;
        public float cooking;
        public float navigation;

        public CrewSkills(float f, float m, float c, float n)
        {
            fighting = f;
            medical = m;
            cooking = c;
            navigation = n;
        }

        public float MultipliedSum(Crew nCrew)
        {
            return nCrew.fighting * fighting + nCrew.medical * medical + nCrew.cooking * cooking + nCrew.navigation * navigation;
        }
    }

    [System.Serializable]
    public struct DataToSerialize
    {
        public int RoomLevel;
        public string RoomUniqueIdentifierForDB;
    }

    public DataToSerialize data;
    // Use this for initialization
    void Start () {
        UpgradeResourceMultiplier = new List<Vector3>()
        {
            new Vector3(1.2f, 1.5f, 1.1f), //bridge
            new Vector3(1.5f, 1.1f, 1.2f), //energy
            new Vector3(1.1f, 1.3f, 1.1f) //food
        };
        crewSkillsResourceMultipliers = new List<CrewSkills>()
        {
            new CrewSkills(1f, 1f, 1f, 1f),
            new CrewSkills(0.8f, 0.5f, 2f, 0.3f)
        };
        
        gameManager = GameManager.instance;
        StartCoroutine(AwaitSetup());
        //GameManager.instance.AddToFirebaseTimedUpdates(this);
    }

    IEnumerator AwaitSetup()
    {
        GameManager.DebugLog("Awaiting user string loading to set up rooms...", DebugFlags.GeneralInfo);
        yield return new WaitUntil(() => !GameManager.instance.user_string.Equals("StillLoading"));
        GameManager.DebugLog("... user string loaded as '" + GameManager.instance.user_string + "', setting up room " + data.RoomUniqueIdentifierForDB, DebugFlags.GeneralInfo);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Rooms/" + data.RoomUniqueIdentifierForDB).ValueChanged += HandleValueChanged;
        //DEBUG_WriteMyRoomData();
    }
	
	// Update is called once per frame
	void Update () {
        if (mainScreen.activeSelf && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) //if there are touch events in the buffer to process...
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), new Vector2(0f, 0f)); //do a raycast to see what they hit
            //Debug.Log(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId));
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                //do nothing
            }
            else if (hit.collider.name == gameObject.name)
            {
                GameManager.DebugLog("Room " + data.RoomUniqueIdentifierForDB + " selected", DebugFlags.CollisionOps); //log that something was hit by the touch event
                Debug.Log(hit.collider);
                currentlySelected = true;
            }
            else
            {
                GameManager.DebugLog("Room " + data.RoomUniqueIdentifierForDB + " no longer selected", DebugFlags.CollisionOps);
                currentlySelected = false;
            }
        }
        Vector3 tempCost = new Vector3(Mathf.Pow(UpgradeResourceMultiplier[(int)RoomType - 1].x, roomLevel), Mathf.Pow(UpgradeResourceMultiplier[(int)RoomType - 1].y, roomLevel), Mathf.Pow(UpgradeResourceMultiplier[(int)RoomType - 1].z, roomLevel));
        upgradeCosts = Vector3.Scale(baseUpgradeCost, tempCost);
        IncreaseResources();
    }

    void ChangeCrew(Crew member)
    {
        //make a check to see if that crew is in that list.
        crewInThisRoom.Remove(member);
    }

    public bool SpacesAvailable()
    {
        if (crewInThisRoom.Count < peopleLimit)
            return true;
        else
        {
            GameManager.DebugLog("Too many people in room " + data.RoomUniqueIdentifierForDB + "\tcurrent: " + crewInThisRoom.Count + " limit: " + peopleLimit, DebugFlags.CollisionOps);
            return false;
        }
    }

    public float SpacesLeft()
    {
        return peopleLimit - crewInThisRoom.Count;
    }

    public void AddPerson(Crew newCrew)
    {
        crewInThisRoom.Add(newCrew);
    }

    public void RemovePerson(Crew crewToRemove)
    {
        crewInThisRoom.Remove(crewToRemove);
    }

    public void CrewIntoPosition()
    {
        for (int i = 0; i < crewInThisRoom.Count; i++)
        {
            if (crewInThisRoom[i] != null)
            {
                if (crewLocations[i] != null)
                {
                    Vector3 oldPosition = crewInThisRoom[i].transform.position;
                    Vector3 newPosition = crewLocations[i].transform.position;
                    crewInThisRoom[i].transform.position = newPosition;
                }
                else
                {
                    GameManager.DebugLog("CrewLocations[" + i + "] is null for room '" + data.RoomUniqueIdentifierForDB + "'!", DebugFlags.CollisionOps);
                }
            }
            else
            {
                GameManager.DebugLog("CrewInThisRoom[" + i + "] is null for room '" + data.RoomUniqueIdentifierForDB + "'!", DebugFlags.CollisionOps);
            }
        }
    }

    void IncreaseResources()
    {
        switch (RoomType)
        {
            case Shared.RoomTypes.food:
                //
                break;
            case Shared.RoomTypes.energy:
                float resourceIncrease = CalculateTotalResourceIncrease(1); //still need to test this
                gameManager.ChangeResource(Shared.ResourceTypes.energy, resourceIncrease);
                break;
            //water
            default: //empty room
                //do nothing
            break;
        }
    }

    bool CrewMoving()
    {
        for (int i = 0; i < crewInThisRoom.Count; i++)
        {
            if(crewInThisRoom[i].GetComponent<DragAndDrop>().selected)
            {
                //Debug.Log("Crew moving");
                return true;
            }
        }
        return false;
    }

    public bool CanIncreaseLevel()
    {
        if (gameManager.GetResource(Shared.ResourceTypes.minerals) > upgradeCosts.x && 
            gameManager.GetResource(Shared.ResourceTypes.energy) > upgradeCosts.y && 
            gameManager.GetResource(Shared.ResourceTypes.money) > upgradeCosts.z)
        {
            return true;
        }
        return false;
    }

    public void IncreaseLevel()
    {
        roomLevel++;
        gameManager.ChangeResource(Shared.ResourceTypes.minerals, -upgradeCosts.x);
        gameManager.ChangeResource(Shared.ResourceTypes.energy, -upgradeCosts.y);
        gameManager.ChangeResource(Shared.ResourceTypes.money, -upgradeCosts.z);
        FirebaseUpdate(false);
    }

    float CalculateTotalResourceIncrease(int roomTypeNum)
    {
        float sum = 1;
        for (int i = 0; i < crewInThisRoom.Count; i++)
        {
            sum += crewSkillsResourceMultipliers[roomTypeNum].MultipliedSum(crewInThisRoom[i]);
        }
        Debug.Log(sum);
        return sum * Time.deltaTime;
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote room " + data.RoomUniqueIdentifierForDB + " with JSON from database: " + json, DebugFlags.DatabaseOps);
        JsonUtility.FromJsonOverwrite(json, data);
    }

    public void DEBUG_WriteMyRoomData()
    {
        string json = JsonUtility.ToJson(data);
        GameManager.DebugLog("[DEBUG] Writing room '" + data.RoomUniqueIdentifierForDB + "' to database. " + json);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Rooms/" + data.RoomUniqueIdentifierForDB).SetRawJsonValueAsync(json);
    }

    public void FirebaseUpdate(bool wasTimedUpdate)
    {
        string json = JsonUtility.ToJson(this);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Rooms/" + data.RoomUniqueIdentifierForDB).SetRawJsonValueAsync(json);
        if (wasTimedUpdate)
            GameManager.DebugLog("[TimedUpdate] Updated room " + data.RoomUniqueIdentifierForDB + " database contents with " + json, DebugFlags.DatabaseOpsOnTimer);
        else
            GameManager.DebugLog("[>TriggeredUpdate] Updated room " + data.RoomUniqueIdentifierForDB + " database contents with " + json, DebugFlags.DatabaseOps);
    }
}
