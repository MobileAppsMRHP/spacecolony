using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour, IFirebaseTimedUpdateable {

    /*public struct RequiredResources
    {
        public float scraps;
        public float energy;
        public float money;

        public RequiredResources(float s, float e, float m)
        {
            scraps = s;
            energy = e;
            money = m;
        }
    }*/
    //public enum RoomType { Food, Bridge, Energy};
    public int peopleLimit;
    public int roomLevel;
    public List<Crew> crewInThisRoom;
    public List<GameObject> crewLocations;
    List<Vector3> UpgradeResourceMultiplier; //mineral (scraps), energy, money
    public Vector3 upgradeCosts;
    public Shared.RoomTypes RoomType;
    public bool currentlySelected;
    private GameManager gameManager;
    List<CrewSkills> crewSkillsResourceMultipliers;

    public string RoomUniqueIdentifierForDB;
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
    }


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
            new CrewSkills(1f, 1f, 1f, 1f)
        };
        gameManager = GameManager.instance;
        StartCoroutine(AwaitSetup());
        //GameManager.instance.AddToFirebaseTimedUpdates(this);
    }

    IEnumerator AwaitSetup()
    {
        GameManager.DebugLog("Awaiting user string loading to set up rooms...");
        yield return new WaitUntil(() => !GameManager.instance.user_string.Equals("StillLoading"));
        GameManager.DebugLog("... user string loaded as '" + GameManager.instance.user_string + "', setting up room " + RoomUniqueIdentifierForDB);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Rooms/" + RoomUniqueIdentifierForDB).ValueChanged += HandleValueChanged;
        //DEBUG_WriteMyRoomData();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) //if there are touch events in the buffer to process...
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), new Vector2(0f, 0f)); //do a raycast to see what they hit
            if (hit.collider.name == gameObject.name) //if it hit something, anything, ....
            {
                Debug.Log("Room selected"); //log that something was hit by the touch event
                Debug.Log(hit.collider);
                currentlySelected = true;
            }
            else
            {
                Debug.Log("Room no longer selected");
                currentlySelected = false;
            }
        }
        Vector3 tempCost = new Vector3(Mathf.Pow(UpgradeResourceMultiplier[(int)RoomType - 1].x, roomLevel), Mathf.Pow(UpgradeResourceMultiplier[(int)RoomType - 1].y, roomLevel), Mathf.Pow(UpgradeResourceMultiplier[(int)RoomType - 1].z, roomLevel));
        upgradeCosts = Vector3.Scale(baseUpgradeCost, tempCost);
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
            //Debug.Log("Too many people in this room");
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

    public void CrewIntoPosition(Crew crewToMove)
    {
        for (int i = 0; i < crewInThisRoom.Count; i++)
        {
            Vector3 oldPosition = crewInThisRoom[i].transform.position;
            Vector3 newPosition = crewLocations[i].transform.position;
            oldPosition = newPosition;
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
                gameManager.resourceManager.ChangeResource(Shared.ResourceTypes.energy, .001f);
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
        if (gameManager.resourceManager.GetResource(Shared.ResourceTypes.minerals) > upgradeCosts.x && 
            gameManager.resourceManager.GetResource(Shared.ResourceTypes.energy) > upgradeCosts.y && 
            gameManager.resourceManager.GetResource(Shared.ResourceTypes.money) > upgradeCosts.z)
        {
            return true;
        }
        return false;
    }

    public void IncreaseLevel()
    {
        roomLevel++;
        //subtract from resources
        FirebaseUpdate(false);
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        string json = args.Snapshot.GetRawJsonValue();
        GameManager.DebugLog("Overwrote room " + RoomUniqueIdentifierForDB + " with JSON from database: " + json, 4);
        JsonUtility.FromJsonOverwrite(json, this);
    }

    public void DEBUG_WriteMyRoomData()
    {
        string json = JsonUtility.ToJson(this);
        GameManager.DebugLog("DEBUG: Writing room '" + RoomUniqueIdentifierForDB + "' to database. " + json);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Rooms/" + RoomUniqueIdentifierForDB).SetRawJsonValueAsync(json);
    }

    public void FirebaseUpdate(bool wasTimedUpdate)
    {
        string json = JsonUtility.ToJson(this);
        FirebaseDatabase.DefaultInstance.GetReference("user-data/" + GameManager.instance.user_string + "/Rooms/" + RoomUniqueIdentifierForDB).SetRawJsonValueAsync(json);
        if (wasTimedUpdate)
            GameManager.DebugLog("[TimedUpdate] Updated room " + RoomUniqueIdentifierForDB + " database contents with " + json, 4);
        else
            GameManager.DebugLog("[>TriggeredUpdate] Updated room " + RoomUniqueIdentifierForDB + " database contents with " + json, 4);
    }
}
