using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GameManagerInstance;
    private List<BuildingsBase> buildings = new List<BuildingsBase>();
    private DateTime focusLostTime;

    private void Awake()
    {
        if (GameManagerInstance is null) GameManagerInstance = this;
        StorageManager.Load();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            StorageManager.Save();
        }
        else
        {
            float elapsedSeconds = (float)(DateTime.UtcNow - focusLostTime).TotalSeconds;

            foreach (BuildingsBase building in buildings)
            {
                building.OfflineProduction(elapsedSeconds);
            }
        }
    }

    public void AddBuildingToList(BuildingsBase building)
    {
        buildings.Add(building);
    }

    private void OnApplicationQuit()
    {
        focusLostTime = DateTime.UtcNow;
        StorageManager.Save();
    }
}