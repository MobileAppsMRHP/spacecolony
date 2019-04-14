using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreScreen : MonoBehaviour {
    public GameObject startButtons;
    public GameManager gameManager;
    public Button buyButton;
    public Button sellButton;
    public Button confirmButton;
    public Text moneyAmountText;
    public Text resourceAmountText;
    float moneyAmount;
    float resourceAmount;
    public enum StoreMode {none, buy, sell};
    public Vector3[] resourceSelectorPositions;
    Shared.ResourceTypes resourceSelected;
    StoreMode storeMode = StoreMode.none;
	// Use this for initialization
	void Start () {
        gameManager = GameManager.instance;
        resourceSelected = Shared.ResourceTypes.energy;
	}
	
	// Update is called once per frame
	void Update () {
        moneyAmount = resourceAmount * 15;
        switch (storeMode)
        {
            case StoreMode.none:
                startButtons.SetActive(true);
                break;
            
            case StoreMode.buy:
                startButtons.SetActive(false);
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
                startButtons.SetActive(false);
                break;
        }
	}

    public void StartStoreScreen()
    {
        storeMode = StoreMode.none;
        moneyAmount = 15f;
        resourceAmount = 1f;
    }

    public void ChangeStoreMode(StoreMode newStoreMode)
    {
        storeMode = newStoreMode;
    }

    public void ChangeValue(float num) //thinking about just making a sell or buy 50 resource button
    {
        resourceAmount += num;
        if (resourceAmount < 1f)
        {
            resourceAmount = 1f;
        }
    }

    public void ChangeResource(Shared.ResourceTypes newResource)
    {
        resourceSelected = newResource;
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
