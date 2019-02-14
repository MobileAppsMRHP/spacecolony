using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class UserAuthentication : MonoBehaviour {
    Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;//import API thorugh default instance of class 
    protected bool fetchingToken = false;                                                                            // Use this for initialization
    public void Start () {
       
    }
	
	// Update is called once per frame
    //using here to exit game when escape key id pressed
	public virtual void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    public void Authenticate()//trades in google token for firebase credentials. made with help from https://firebase.google.com/docs/auth/unity/google-signin?authuser=0
    {
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
     public void SignOut( Firebase.Auth.FirebaseAuth auth)//signs out user 
    {
        auth.SignOut();
    }

    public string getUsername(Firebase.Auth.FirebaseUser user)//returns the signed in users username
    {
        return user.DisplayName; 
    }

    public void TrackTokenChanges(object sender, System.EventArgs eventargs)//tracks changes to user auth token 
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        if (senderAuth == auth && fetchingToken==false)
        {
            senderAuth.CurrentUser.TokenAsync(false).ContinueWith(Task => print(string.Format("Token[0:8] = {0}", Task.Result.Substring(0, 8))));
         }
    }

  //eventually we can use the method below to sign in with google or other provider credntials
    /* public Task SigninWithEmailCredentialAsync()
    {
        if (signInAndFetchProfile)
        {
            return auth.SignInAndRetrieveDataWithCredentialAsync(
              Firebase.Auth.EmailAuthProvider.GetCredential(email, password)).ContinueWith(
                HandleSignInWithSignInResult);
        }
        else
        {
            return auth.SignInWithCredentialAsync(
              Firebase.Auth.EmailAuthProvider.GetCredential(email, password)).ContinueWith(
                HandleSignInWithUser);
        }
    }*/
}
