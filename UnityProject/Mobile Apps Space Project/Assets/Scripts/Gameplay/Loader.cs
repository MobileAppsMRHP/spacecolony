using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    public GameManager currentGameManager;
    // Use this for initialization
    void Awake()
    {
        if (GameManager.instance == null)
        {
            Debug.Log("Created new gamemanager");
            currentGameManager = Instantiate(gameManager).GetComponent<GameManager>();
        }
    }
}
	
