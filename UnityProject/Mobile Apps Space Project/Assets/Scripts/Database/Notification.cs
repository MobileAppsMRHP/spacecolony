using System.Collections;
using System.Collections.Generic;
using Firebase.Messaging;
using UnityEngine;

public class Notification : MonoBehaviour {

	
	public void Start ()
    {
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;
    }
	
	
	public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received Regestration Token: " + token.Token);
	}

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message form: " + e.Message.From);
    }
}
