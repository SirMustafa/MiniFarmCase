using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private DateTime focusLostTime;
    private bool hasRecorded = false;

    private void Awake()
    {
        StorageManager.Load();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            focusLostTime = DateTime.UtcNow;
            hasRecorded = true;
            StorageManager.Save();
        }
        else if (hasFocus && hasRecorded)
        {
            DateTime focusGainedTime = DateTime.UtcNow;
            TimeSpan elapsed = focusGainedTime - focusLostTime;
            hasRecorded = false;
        }
    }

    private void OnApplicationQuit()
    {
        StorageManager.Save();
    }
}