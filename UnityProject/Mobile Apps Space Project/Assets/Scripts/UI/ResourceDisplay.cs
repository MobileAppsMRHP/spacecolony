using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour {
    public List<Text> resources;
    public GameManager gameManager;
    //ResourceManager resourceManager;

	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
        //resourceManager = gameManager.resourceManager;
        if (GameManager.IsDoneLoading)
        {
            for (int i = 0; i < resources.Count; i++)
            {
                resources[i].text = Mathf.Round(gameManager.GetResource((Shared.ResourceTypes)i)).ToString();
            }
            /*resources[0].text = Mathf.Round(gameManager.GetResource(Shared.ResourceTypes.minerals)).ToString();
            resources[1].text = Mathf.Round(gameManager.GetResource(Shared.ResourceTypes.energy)).ToString();
            resources[2].text = Mathf.Round(gameManager.GetResource(Shared.ResourceTypes.money)).ToString();
            resources[3].text = Mathf.Round(gameManager.GetResource(Shared.ResourceTypes.food)).ToString();
            resources[4].text = Mathf.Round(gameManager.GetResource(Shared.ResourceTypes.water)).ToString();*/
        }   
    }


}
