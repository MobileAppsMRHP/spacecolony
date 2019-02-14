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
	
	
	public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Received Regestration Token: " + token.Token);
	}

    public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message form: " + e.Message.From);
    }
}
