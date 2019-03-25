using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontFixer : MonoBehaviour {

    public Font[] fonts;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < fonts.Length; i++)
        {
            fonts[i].material.mainTexture.filterMode = FilterMode.Point;
        }
	}
}
