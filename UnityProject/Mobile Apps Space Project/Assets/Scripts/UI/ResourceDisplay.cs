﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour {
    public List<Text> resources;
    public GameManager gameManager;
    ResourceManager resourceManager;

	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
        resourceManager = gameManager.resourceManager;
        resources[0].text = resourceManager.GetResource(Shared.ResourceTypes.scraps).ToString();
        resources[1].text = resourceManager.GetResource(Shared.ResourceTypes.energy).ToString();
        resources[2].text = resourceManager.GetResource(Shared.ResourceTypes.money).ToString();
    }


}