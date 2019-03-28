using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFirebaseTimedUpdateable {

    void FirebaseUpdate(bool wasTimedUpdate);
}
