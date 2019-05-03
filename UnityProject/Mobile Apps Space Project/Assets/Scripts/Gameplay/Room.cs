using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Room : MonoBehaviour {

    public int peopleLimit;
    public int roomLevel;
    public List<Crew> crewInThisRoom;
    public List<GameObject> crewLocations;
    List<Vector3> UpgradeResourceMultiplier; //mineral, energy, money
    public Vector3 upgradeCosts;
    public Shared.RoomTypes RoomType;
    public bool currentlySelected;
    private GameManager gameManager;
    public GameObject mainScreen;
    List<CrewSkills> crewSkillsResourceMultipliers;
    public bool isPlanet;
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
            return nCrew.AllData.SkillData.Skill_Fighting * fighting + nCrew.AllData.SkillData.Skill_Medical * medical + nCrew.AllData.SkillData.Skill_Cooking * cooking + nCrew.AllData.SkillData.Skill_Navigation * navigation;
        }
    }

    [System.Serializable]
    public struct DataToSerialize
    {
        public int RoomLevel;
    }

    [System.Serializable]
    public struct PlanetImages
    {
        public Sprite DefaultPlanet;
        public Sprite FoodPlanet;
        public Sprite EnergyPlanet;
        public Sprite MineralPlanet;
        public Sprite WaterPlanet;
        
    }

    public string RoomUniqueIdentifierForDB;

    public PlanetImages PlanetImagesData;
    public DataToSerialize data;
    // Use this for initialization
    void Start () {
        UpgradeResourceMultiplier = new List<Vector3>()
        {
            new Vector3(0f, 0f, 0f), //none
            new Vector3(1.2f, 1.5f, 1.1f), //bridge
            new Vector3(1.5f, 1.1f, 1.2f), //energy
            new Vector3(1.1f, 1.3f, 1.1f), //food
            new Vector3(1.4f, 1.3f, 1.6f), //minerals
            new Vector3(1.5f, 1.3f, 1.7f) //water
        };
        crewSkillsResourceMultipliers = new List<CrewSkills>()
        {
            new CrewSkills(0f, 0f, 0f, 0f), //none
            new CrewSkills(1f, 1f, 1f, 1f), //bridge
            new CrewSkills(0.8f, 0.5f, 2f, 0.3f), //energy
            new CrewSkills(0.1f, 0.2f, 0.5f, 0.1f), //food
            new CrewSkills(0.4f, 0.3f, 0.8f, 0.9f), //minerals
            new CrewSkills(0.2f, 0.2f, 0.2f, 0.2f) //water
        };
        
        gameManager = GameManager.instance;
        StartCoroutine(AwaitSetup());
        //GameManager.instance.AddToFirebaseTimedUpdates(this);
    }

    IEnumerator AwaitSetup()
    {
        GameManager.DebugLog("Room " + RoomUniqueIdentifierForDB + " awaiting user string loading to set up rooms...", DebugFlags.GeneralInfo);
        yield return new WaitUntil(() => !GameManager.instance.user_string.Equals("StillLoading"));
        GameManager.DebugLog("... user string loaded as '" + GameManager.instance.user_string + "', setting up room " + RoomUniqueIdentifierForDB, DebugFlags.GeneralInfo);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Rooms/" + RoomUniqueIdentifierForDB).ValueChanged += HandleValueChanged;
        if(isPlanet)
        {
            //Debug.Log("A planet was registered");
            FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/CurrentPlanet").ValueChanged += HandlePlanetChanged;
        }
        //DEBUG_WriteMyRoomData();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 tempCost;
        if (GameManager.IsDoneLoading)
        {
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
                    GameManager.DebugLog("Room " + RoomUniqueIdentifierForDB + " selected", DebugFlags.CollisionOps); //log that something was hit by the touch event
                    Debug.Log(hit.collider);
                    currentlySelected = true;
                }
                else
                {
                    GameManager.DebugLog("Room " + RoomUniqueIdentifierForDB + " no longer selected", DebugFlags.CollisionOps);
                    currentlySelected = false;
                }
            }
            if (!(RoomType == Shared.RoomTypes.empty))
            {
                tempCost = new Vector3(Mathf.Pow(UpgradeResourceMultiplier[(int)RoomType].x, roomLevel), Mathf.Pow(UpgradeResourceMultiplier[(int)RoomType].y, roomLevel), Mathf.Pow(UpgradeResourceMultiplier[(int)RoomType].z, roomLevel));
                upgradeCosts = Vector3.Scale(baseUpgradeCost, tempCost);
                IncreaseResources();
            }
            else
            {
                currentlySelected = false;
            }
        }
        if (isPlanet)
        {
            currentlySelected = false;
        }
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
            GameManager.DebugLog("Too many people in room " + RoomUniqueIdentifierForDB + "\tcurrent: " + crewInThisRoom.Count + " limit: " + peopleLimit, DebugFlags.CollisionOps);
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
                    GameManager.DebugLog("CrewLocations[" + i + "] is null for room '" + RoomUniqueIdentifierForDB + "'!", DebugFlags.CollisionOps);
                }
            }
            else
            {
                GameManager.DebugLog("CrewInThisRoom[" + i + "] is null for room '" + RoomUniqueIdentifierForDB + "'!", DebugFlags.CollisionOps);
            }
        }
    }

    void IncreaseResources()
    {
        float resourceIncrease = 0;
        switch (RoomType)
        {
            case Shared.RoomTypes.food:
                resourceIncrease = CalculateTotalResourceIncrease((int)Shared.RoomTypes.food);
                gameManager.ChangeResource(Shared.ResourceTypes.food, resourceIncrease);
                break;
            case Shared.RoomTypes.energy:
                resourceIncrease = CalculateTotalResourceIncrease((int)Shared.RoomTypes.energy); 
                gameManager.ChangeResource(Shared.ResourceTypes.energy, resourceIncrease);
                break;
            case Shared.RoomTypes.water:
                resourceIncrease = CalculateTotalResourceIncrease((int)Shared.RoomTypes.water);
                gameManager.ChangeResource(Shared.ResourceTypes.water, resourceIncrease);
                break;
            case Shared.RoomTypes.mineral:
                resourceIncrease = CalculateTotalResourceIncrease((int)Shared.RoomTypes.mineral);
                gameManager.ChangeResource(Shared.ResourceTypes.minerals, resourceIncrease);
                break;
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
        //FirebaseUpdate(false);
    }

    float CalculateTotalResourceIncrease(int roomTypeNum)
    {
        float sum = 0;
        for (int i = 0; i < crewInThisRoom.Count; i++)
        {
            sum += crewSkillsResourceMultipliers[roomTypeNum].MultipliedSum(crewInThisRoom[i]);
        }
        //Debug.Log(RoomType + "" + sum);
        if (isPlanet)
        {
            sum = sum * 20;
        }
        return sum * roomLevel/10f * Time.deltaTime;
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote room " + RoomUniqueIdentifierForDB + " with JSON from database: " + json, DebugFlags.DatabaseOps);
        object boxedDataCloneForJsonUtility = data; //needs special boxing because https://docs.unity3d.com/ScriptReference/EditorJsonUtility.FromJsonOverwrite.html
        JsonUtility.FromJsonOverwrite(json, boxedDataCloneForJsonUtility);
        data = (DataToSerialize)boxedDataCloneForJsonUtility;
    }

    void HandlePlanetChanged(object sender, ValueChangedEventArgs args)
    {
        //Debug.LogError("Received planet value " + );
        if (args.Snapshot.Value == null)
        {
            Debug.LogWarning("No planet value received, using mineral planet by default");
            RoomType = Shared.RoomTypes.mineral;
        }
        else
        {
            RoomType = (Shared.RoomTypes)System.Enum.Parse(typeof(Shared.RoomTypes), args.Snapshot.Value.ToString());
            GameManager.DebugLog("Planet changed to " + RoomType + "(" + args.Snapshot.Value + ")");
        }
        UpdateSprite();
    }

    void UpdateSprite()
    {
        Sprite temp = PlanetImagesData.DefaultPlanet;
        switch (RoomType)
        {
            case Shared.RoomTypes.empty:
                temp = PlanetImagesData.DefaultPlanet;
                break;
            case Shared.RoomTypes.bridge:
                temp = PlanetImagesData.DefaultPlanet;
                break;
            case Shared.RoomTypes.energy:
                temp = PlanetImagesData.EnergyPlanet;
                break;
            case Shared.RoomTypes.food:
                temp = PlanetImagesData.FoodPlanet;
                break;
            case Shared.RoomTypes.mineral:
                temp = PlanetImagesData.MineralPlanet;
                break;
            case Shared.RoomTypes.water:
                temp = PlanetImagesData.WaterPlanet;
                break;
            default:
                Debug.LogError("Unexpected room type when updating planet image!");
                break;
        }
        Debug.Log("Switched planet sprite to " + temp.name);
        transform.GetComponent<SpriteRenderer>().sprite = temp;
        
    }

    public void NewUser_WriteMyRoomData()
    {
        string json = JsonUtility.ToJson(data);
        GameManager.DebugLog("[NewUser] Writing room '" + RoomUniqueIdentifierForDB + "' to database. " + json);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Rooms/" + RoomUniqueIdentifierForDB).SetRawJsonValueAsync(json);
    }

    /*public void FirebaseUpdate(bool wasTimedUpdate)
    {
        Debug.Log("Deprecated method called");
        string json = JsonUtility.ToJson(this);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Rooms/" + data.RoomUniqueIdentifierForDB).SetRawJsonValueAsync(json);
        if (wasTimedUpdate)
            GameManager.DebugLog("[TimedUpdate] Updated room " + data.RoomUniqueIdentifierForDB + " database contents with " + json, DebugFlags.DatabaseOpsOnTimer);
        else
            GameManager.DebugLog("[>TriggeredUpdate] Updated room " + data.RoomUniqueIdentifierForDB + " database contents with " + json, DebugFlags.DatabaseOps);
    }*/
}
