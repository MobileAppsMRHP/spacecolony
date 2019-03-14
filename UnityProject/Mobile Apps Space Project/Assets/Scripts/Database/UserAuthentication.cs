using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class UserAuthentication : MonoBehaviour {
    Firebase.Auth.FirebaseAuth auth;
    protected bool fetchingToken = false;
    protected bool signInAndFetchProfile = true;
    protected string email = "";
    protected string password = "";
    protected Firebase.Auth.Credential credential;
    // The verification id needed along with the sent code for phone authentication.
    private string phoneAuthVerificationId;

    // Use this for initialization
     void Awake() {
       auth = Firebase.Auth.FirebaseAuth.DefaultInstance;//import API thorugh default instance of class 
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

  //eventually we can use the method below to sign in with google or other provider credentials
    public Task SigninWithEmailCredentialAsync()
    {
        if (signInAndFetchProfile)
        {
            return auth.SignInAndRetrieveDataWithCredentialAsync(
              credential=Firebase.Auth.GoogleAuthProvider.GetCredential(email, password)).ContinueWith(HandleSignInWithSignInResult);
        }
        else
        {
            return auth.SignInWithCredentialAsync(
              Firebase.Auth.GoogleAuthProvider.GetCredential(email, password)).ContinueWith(
                HandleSignInWithUser);
        }
    }

    // Called when a sign-in without fetching profile data completes.
    void HandleSignInWithUser(Task<Firebase.Auth.FirebaseUser> task)
    {
        if (task.IsCompleted)
        {
            print(string.Format("{0} signed in", task.Result.DisplayName));
        }
    }

    // Called when a sign-in with profile data completes.
    void HandleSignInWithSignInResult(Task<Firebase.Auth.SignInResult> task)
    {
        if (task.IsCompleted)
        {
            DisplaySignInResult(task.Result, 1);
        }
    }

    // Display user information reported by the firebase auth sign in
    protected void DisplaySignInResult(Firebase.Auth.SignInResult result, int indentLevel)
    {
        string indent = new string(' ', indentLevel * 2);
        var metadata = result.Meta;
        if (metadata != null)
        {
            print(string.Format("{0}Created: {1}", indent, metadata.CreationTimestamp));
            print(string.Format("{0}Last Sign-in: {1}", indent, metadata.LastSignInTimestamp));
        }
        var info = result.Info;
        if (info != null)
        {
            print(string.Format("{0}Additional User Info:", indent));
            print(string.Format("{0}  User Name: {1}", indent, info.UserName));
            print(string.Format("{0}  Provider ID: {1}", indent, info.ProviderId));
            DisplayProfile<string>(info.Profile, indentLevel + 1);
        }
    }

    // Display additional user profile information.
    protected void DisplayProfile<T>(IDictionary<T, object> profile, int indentLevel)
    {
        string indent = new string(' ', indentLevel * 2);
        foreach (var kv in profile)
        {
            var valueDictionary = kv.Value as IDictionary<object, object>;
            if (valueDictionary != null)
            {
                print(string.Format("{0}{1}:", indent, kv.Key));
                DisplayProfile<object>(valueDictionary, indentLevel + 1);
            }
            else
            {
                print(string.Format("{0}{1}: {2}", indent, kv.Key, kv.Value));
            }
        }
    }

    public Firebase.Auth.Credential getCredential()
    {
        return credential;
    }

}
