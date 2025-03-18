using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HayFactory : BuildingsBase
{
    [SerializeField] private float productionTime = 40f;
    [SerializeField] private int capacity = 10;

    private bool isProducingFlag = false;
    public override bool IsProducing => isProducingFlag;

    public override float ProductionTime => productionTime;
    public override int Capacity => capacity;

    private void Start()
    {
        ProduceResources();
    }

    protected override async void ProduceResources()
    {
        if (isProducingFlag) return;
        isProducingFlag = true;
        while (InternalStorage.Value < Capacity)
        {
            float timer = 0f;
            while (timer < ProductionTime)
            {
                ProductionProgress.Value = timer / ProductionTime;
                await UniTask.Yield();
                timer += Time.deltaTime;
            }
            ProductionProgress.Value = 0f;
            InternalStorage.Value++;
        }
        isProducingFlag = false;
    }

    public override void CollectResources()
    {
        Storage.WheatCount.Value += InternalStorage.Value;
        InternalStorage.Value = 0;
    }

    public override void EnqueueProductionOrder() { }
    public override void CancelProductionOrder() { }
}