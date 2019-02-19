using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class SettingsButton : MonoBehaviour
    {
        public void OpenSettings()
        {
            SceneManager.LoadScene("02Settings");
        }

        public void QuitSettings()
        {
        SceneManager.LoadSceneAsync("01Gameplay");
        }
    }
