using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public static class StorageManager
{
    public enum ResourceType
    {
        Wheat,
        Flour,
        Bread
    }

    private static readonly Dictionary<ResourceType, ReactiveProperty<int>> resourceStorage = new Dictionary<ResourceType, ReactiveProperty<int>>();

    static StorageManager()
    {
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            resourceStorage[type] = new ReactiveProperty<int>(0);
        }
    }

    public static void AddResource(ResourceType type, int amount)
    {
        resourceStorage[type].Value += amount;
    }

    public static void RemoveResource(ResourceType type, int amount)
    {
        resourceStorage[type].Value = Mathf.Max(0, resourceStorage[type].Value - amount);
    }

    public static int GetResourceCount(ResourceType type)
    {
        return resourceStorage.ContainsKey(type) ? resourceStorage[type].Value : 0;
    }

    public static ReactiveProperty<int> GetResourceProperty(ResourceType type)
    {
        return resourceStorage.ContainsKey(type) ? resourceStorage[type] : new ReactiveProperty<int>(0);
    }

    public static async void Save()
    {
        foreach (var resource in resourceStorage)
        {
            PlayerPrefs.SetInt(resource.Key.ToString(), resource.Value.Value);
        }
        PlayerPrefs.Save();
        await UniTask.Delay(1);
    }

    public static async void Load()
    {
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            resourceStorage[type].Value = PlayerPrefs.GetInt(type.ToString(), 0);
        }
        await UniTask.Delay(1);
    }
}