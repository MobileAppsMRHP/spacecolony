using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceChange : MonoBehaviour {
    GameManager gameManager;
    ResourceManager resourceManager;
	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
        resourceManager = gameManager.resourceManager;
    }

    public void AddOneToEach()
    {
        resourceManager.ChangeResource(Shared.ResourceTypes.minerals, 1f);
        resourceManager.ChangeResource(Shared.ResourceTypes.energy, 1f);
        resourceManager.ChangeResource(Shared.ResourceTypes.water, 1f);
        resourceManager.ChangeResource(Shared.ResourceTypes.money, 1f);
        resourceManager.ChangeResource(Shared.ResourceTypes.food, 1f);
    }
}
