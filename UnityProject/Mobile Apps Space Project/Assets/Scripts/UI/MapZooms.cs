using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapZooms : MonoBehaviour {

    public Camera MainCamera;
    public GameObject TargetPosition;
    public int speed = 2;
    public int camx;
    public int camy;
    public int camz;
    bool camera_move_enabled = false;

    void Update()
    {

        if (camera_move_enabled)
        {
            MainCamera.transform.position = Vector3.Lerp(transform.position, TargetPosition.transform.position, speed * Time.deltaTime);
        }

    }

    public void UserClickedCameraResetButton()
    {
        TargetPosition.transform.position = new Vector3(camx, camy, camz);
        camera_move_enabled = false;
    }
}
