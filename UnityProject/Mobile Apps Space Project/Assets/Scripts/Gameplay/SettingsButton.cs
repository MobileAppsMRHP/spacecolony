﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class SettingsButton : MonoBehaviour
    {
        public void OpenSettings()
        {
            SceneManager.LoadScene("02Settings");
        }

        public void GameScreen()
        {
        SceneManager.LoadScene("01Gameplay");
        }

    public void TitleScreen()
    {
        SceneManager.LoadScene("00TitleScreen");
    }
}
