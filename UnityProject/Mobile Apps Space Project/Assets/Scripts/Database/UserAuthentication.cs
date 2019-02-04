using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAuthentication : MonoBehaviour {

	// Use this for initialization
	public void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Authenticate()//trades in google token for firebase credentials. made with help from https://firebase.google.com/docs/auth/unity/google-signin?authuser=0
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;//import API thorugh default instance of class 
        Firebase.Auth.Credential credential = //REPLACE THESE ACCESS TOKENS LATER
            Firebase.Auth.GoogleAuthProvider.GetCredential("googleIdToken", "googleAccessToken");
            auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
            });
    }
     public void signOut( Firebase.Auth.FirebaseAuth auth)//signs out user 
    {
        auth.SignOut();
    }

    public string getUsername(Firebase.Auth.FirebaseUser user)//returns the signed in users username
    {
        return user.DisplayName; 
    }
}
