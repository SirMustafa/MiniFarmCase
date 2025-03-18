using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class BuildingsBase : MonoBehaviour, IClickable
{
    public ReactiveProperty<int> InternalStorage { get; private set; } = new ReactiveProperty<int>(0);
    public ReactiveProperty<float> ProductionProgress { get; private set; } = new ReactiveProperty<float>(0f);
    public bool isPanelOpened;
    public abstract float ProductionTime { get; }
    public abstract int Capacity { get; }
    public abstract void EnqueueProductionOrder();
    public abstract void CancelProductionOrder();
    protected abstract void ProduceResources();

    public virtual void OnClick()
    {
        UiManager.UiManagerInstance.ShowBuildingPanel(this);
    }
}