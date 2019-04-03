using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableLoginUI : MonoBehaviour {

    protected static UserAuthentication auth;

    // Use this for initialization
    void Start () {
        auth = gameObject.AddComponent<UserAuthentication>();
        auth.EnableUI();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

}
