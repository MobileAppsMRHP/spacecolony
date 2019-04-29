using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMapScreenScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SwitchToMapScreen()
    {
        Debug.Log("Loading map scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2, LoadSceneMode.Additive);
        /*Debug.Log("Unloading scene " + SceneManager.GetActiveScene().name);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());*/
    }

    public void MapScreenToMainScreen()
    {
        Debug.Log("Unloading map scene");
        SceneManager.UnloadSceneAsync("03Map");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2, LoadSceneMode.Additive);
        /*Debug.Log("Unloading scene " + SceneManager.GetActiveScene().name);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());*/
    }
}
