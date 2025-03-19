using UniRx;
using UnityEngine;

public static class Storage
{
    public static ReactiveProperty<int> WheatCount { get; } = new ReactiveProperty<int>(0);
    public static ReactiveProperty<int> FlourCount { get; } = new ReactiveProperty<int>(0);
    public static ReactiveProperty<int> BreadCount { get; } = new ReactiveProperty<int>(0);
    public static void Save()
    {
        PlayerPrefs.SetInt("WheatCount", WheatCount.Value);
        PlayerPrefs.SetInt("FlourCount", FlourCount.Value);
        PlayerPrefs.SetInt("BreadCount", BreadCount.Value);
        PlayerPrefs.Save();
    }

    public static void Load()
    {
        WheatCount.Value = PlayerPrefs.GetInt("WheatCount", WheatCount.Value);
        FlourCount.Value = PlayerPrefs.GetInt("FlourCount", FlourCount.Value);
        BreadCount.Value = PlayerPrefs.GetInt("BreadCount", BreadCount.Value);
    }
}