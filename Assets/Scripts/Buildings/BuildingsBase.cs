using Cysharp.Threading.Tasks;
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

    public abstract void CollectResources();

    public abstract bool IsProducing { get; }

    public virtual void OnClick()
    {
        if (!isPanelOpened)
        {
            UiManager.UiManagerInstance.ShowBuildingPanel(this);
            isPanelOpened = true;
        }
        else
        {
            CollectResources();
            if (!IsProducing)
            {
                ProduceResources();
            }
        }
    }

    protected async UniTask ProcessProductionQueue(
        ReactiveProperty<int> productionQueue,
        ReactiveProperty<int> inputResource,
        int costPerOrder,
        ReactiveProperty<int> outputResource)
    {
        while (productionQueue.Value > 0)
        {
            if (inputResource.Value < costPerOrder)
                break;
            inputResource.Value -= costPerOrder;

            float timer = 0f;
            while (timer < ProductionTime)
            {
                ProductionProgress.Value = timer / ProductionTime;
                await UniTask.Yield();
                timer += Time.deltaTime;
            }
            ProductionProgress.Value = 0f;
            outputResource.Value++;
            InternalStorage.Value++;
            productionQueue.Value--;
        }
    }
}