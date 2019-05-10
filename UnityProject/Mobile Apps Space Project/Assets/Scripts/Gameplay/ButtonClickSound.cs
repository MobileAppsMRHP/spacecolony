using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickSound : MonoBehaviour {

    // DEPRECATED - DO NOT USE THIS. Instead use GameManager.PlayButtonSound()

    //public AudioClip soundToPlay;
    private AudioSource source;

	// Use this for initialization
	void Start () {
        source = gameObject.AddComponent<AudioSource>();//.playOnAwake = false;
        source.playOnAwake = false;
        source.clip = Resources.Load<AudioClip>("ButtonClick");
        GameManager.DebugLog("Added an audio souce to " + name, DebugFlags.SoundSystem);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Play()
    {
        GameManager.DebugLog("Playing button click sound", DebugFlags.SoundSystem);
        source.Play();
    }
}
