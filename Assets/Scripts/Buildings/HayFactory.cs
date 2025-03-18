using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HayFactory : BuildingsBase
{
    [SerializeField] private float productionTime; 
    [SerializeField] private int capacity; 
    private bool isProducing; 
    public override float ProductionTime => productionTime;
    public override int Capacity => capacity;

    private void Start()
    {
        ProduceResources();
    }

    protected override async void ProduceResources()
    {
        if (isProducing) return;
        isProducing = true;

        while (InternalStorage.Value < capacity)
        {
            float timer = 0f;

            while (timer < productionTime)
            {
                ProductionProgress.Value = timer / productionTime;
                await UniTask.Yield();
                timer += Time.deltaTime;
            }

            InternalStorage.Value++;
            ProductionProgress.Value = 0f;
        }

        isProducing = false;
    }

    public override void OnClick()
    {
        if (!isPanelOpened)
        {
            base.OnClick();
            isPanelOpened = true;
        }
        else
        {
            CollectResources();

            if (!isProducing)
            {
                ProduceResources();
            }
        }
    }

    public void CollectResources()
    {
        Storage.WheatCount.Value += InternalStorage.Value;
        InternalStorage.Value = 0;
    }

    public override void EnqueueProductionOrder()
    {
    }

    public override void CancelProductionOrder()
    {
    }
}