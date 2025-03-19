using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class BuildingsBase : MonoBehaviour, IClickable
{
    public ReactiveProperty<int> InternalStorage { get; } = new ReactiveProperty<int>(0);
    public ReactiveProperty<float> ProductionProgress { get; } = new ReactiveProperty<float>(0f);
    public bool IsPanelOpened { get; set; }

    public abstract float ProductionTime { get; }
    public abstract int Capacity { get; }
    public abstract int OutPutResourceAmount { get; }
    public abstract bool IsProducing { get; }

    public abstract void EnqueueProductionOrder();
    public abstract void DequeueProductionOrder();
    protected abstract void ProduceResources();
    public abstract void CollectResources();

    public virtual void OnClick()
    {
        if (!IsPanelOpened)
        {
            UiManager.UiManagerInstance.ShowBuildingPanel(this);
            IsPanelOpened = true;
        }
        else
        {
            CollectResources();
            if (!IsProducing)
                ProduceResources();
        }
    }

    protected async UniTask ProcessProductionQueue(ReactiveProperty<int> productionQueue, ReactiveProperty<int> inputResource, int costPerOrder)
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
            InternalStorage.Value++;
            productionQueue.Value--;
        }
    }
}