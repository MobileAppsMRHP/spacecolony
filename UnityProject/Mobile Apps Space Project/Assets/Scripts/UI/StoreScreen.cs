using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreScreen : MonoBehaviour {
    public GameManager gameManager;
    public Button confirmButton;
    public Text moneyAmountText;
    public Text resourceAmountText;
    float moneyAmount;
    float resourceAmount = 1;
    public enum StoreMode {none, buy, sell};
    public Vector3[] resourceSelectorPositions;
    Shared.ResourceTypes resourceSelected;
    StoreMode storeMode = StoreMode.none;
    public GameObject[] resourceButtonLocations;
    public GameObject resourceHighlight;
    public GameObject buyMode;
    public GameObject sellMode;
    public GameObject commonMode;
    public GameObject storeStarter;
	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
        resourceSelected = Shared.ResourceTypes.minerals;
	}
	
	// Update is called once per frame
	void Update () {
        moneyAmount = resourceAmount * 15;
        moneyAmountText.text = moneyAmount.ToString();
        resourceAmountText.text = resourceAmount.ToString();
        switch (storeMode)
        {
            case StoreMode.none:
                buyMode.SetActive(false);
                sellMode.SetActive(false);
                commonMode.SetActive(false);
                storeStarter.SetActive(true);
                break;
            
            case StoreMode.buy:
                if (gameManager.GetResource(Shared.ResourceTypes.money) < moneyAmount)
                    confirmButton.interactable = false;
                else
                    confirmButton.interactable = true;
                break;

            case StoreMode.sell:
                if (gameManager.GetResource(resourceSelected) < resourceAmount)
                    confirmButton.interactable = false;
                else
                    confirmButton.interactable = true;
                break;
        }

	}

    public void StartStoreScreen()
    {
        storeMode = StoreMode.none;
        ChangeResource(0);
        moneyAmount = 15f;
        resourceAmount = 1f;
    }

    public void ChangeStoreMode(int num)//StoreMode newStoreMode
    {
        storeMode = (StoreMode)num;
    }

    public void ChangeValue(float num) 
    {
        Debug.Log("changing value by " + num);
        resourceAmount += num;
        if (resourceAmount < 1f)
        {
            resourceAmount = 1f;
        }
    }

    public void ChangeResource(int num)
    {
        resourceSelected = (Shared.ResourceTypes)num;
        Debug.Log("switching resources to " + (Shared.ResourceTypes)num);
        if (num == 4)
        {
            resourceHighlight.transform.position = resourceButtonLocations[3].transform.position;
        }
        else
        {
            resourceHighlight.transform.position = resourceButtonLocations[num].transform.position;
        }
    }

    public void ConfirmTransaction()
    {
        if (storeMode == StoreMode.buy)
        {
            gameManager.ChangeResource(resourceSelected, resourceAmount);
            gameManager.ChangeResource(Shared.ResourceTypes.money, -1 * moneyAmount);
        }
        else
        {
            gameManager.ChangeResource(resourceSelected, -1 * resourceAmount);
            gameManager.ChangeResource(Shared.ResourceTypes.money, moneyAmount);
        }
    }
}
