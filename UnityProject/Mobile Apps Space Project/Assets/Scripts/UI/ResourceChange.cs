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

    public void AddToEach(float n)
    {
        gameManager.ChangeResource(Shared.ResourceTypes.minerals, n);
        gameManager.ChangeResource(Shared.ResourceTypes.energy, n);
        gameManager.ChangeResource(Shared.ResourceTypes.water, n);
        gameManager.ChangeResource(Shared.ResourceTypes.money, n);
        gameManager.ChangeResource(Shared.ResourceTypes.food, n);
    }
}
