using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceChange : MonoBehaviour {
    GameManager gameManager;
    //ResourceManager resourceManager;
	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public void AddOneToEach()
    {
        gameManager.ChangeResource(Shared.ResourceTypes.minerals, 1f);
        gameManager.ChangeResource(Shared.ResourceTypes.energy, 1f);
        gameManager.ChangeResource(Shared.ResourceTypes.water, 1f);
        gameManager.ChangeResource(Shared.ResourceTypes.money, 1f);
        gameManager.ChangeResource(Shared.ResourceTypes.food, 1f);
    }
}
