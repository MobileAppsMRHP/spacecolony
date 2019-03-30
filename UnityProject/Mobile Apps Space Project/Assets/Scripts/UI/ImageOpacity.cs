using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageOpacity : MonoBehaviour {
    public float opacity;
    Image image;
    Color imageColor;
    // Use this for initialization
    void Start () {
        image = GetComponent<Image>();
        imageColor = image.color;
	}
	
	// Update is called once per frame
	void Update () {
        imageColor.a = opacity;
        image.color = imageColor;
	}
}
