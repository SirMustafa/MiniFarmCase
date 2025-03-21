using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public void AddBuildingToList(BuildingsBase building)
    {
        buildings.Add(building);
    }
    public void testt(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Debug.Log("hopp");
        focusLostTime = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(4));
        float offlineSeconds = (float)(DateTime.UtcNow - focusLostTime).TotalSeconds;

        foreach (BuildingsBase building in buildings)
        {
            building.OfflineProduction(offlineSeconds);
        }

    }
    private void OnApplicationPause(bool hasFocus)
    {
        if (!hasFocus)
        {
            
            focusLostTime = DateTime.UtcNow;
            StorageManager.Save();
        }
        else if (hasFocus)
        {
            Debug.Log("poh");
            DateTime focusGainedTime = DateTime.UtcNow;
            TimeSpan elapsed = focusGainedTime - focusLostTime;
            float offlineSeconds = (float)elapsed.TotalSeconds;

            foreach (BuildingsBase building in buildings)
            {
                building.OfflineProduction(offlineSeconds);
            }
        }
    }

    private void OnApplicationQuit()
    {
        StorageManager.Save();
    }
}