using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GameManagerInstance;
    private List<BuildingsBase> buildings = new List<BuildingsBase>();
    private DateTime focusLostTime;
    private bool hasRecorded = false;

    private void Awake()
    {
        if(GameManagerInstance is null)  GameManagerInstance = this;
        StorageManager.Load();
    }

    public void AddBuildingToList(BuildingsBase building)
    {
        buildings.Add(building);
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
            float offlineSeconds = (float)elapsed.TotalSeconds;

            foreach (BuildingsBase building in buildings)
            {
                building.ApplyOfflineProduction(offlineSeconds);
            }

            hasRecorded = false;
        }
    }

    private void OnApplicationQuit()
    {
        StorageManager.Save();
    }
}