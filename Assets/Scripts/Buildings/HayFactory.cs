using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class HayFactory : BuildingsBase
{
    [Header("BuildStats")]
    [SerializeField] private float productionTime = 40f;
    [SerializeField] private int capacity = 10;
    [SerializeField] private int _output = 1;

    private bool isProducingFlag = false;
    public override bool IsProducing => isProducingFlag;
    public override float ProductionTime => productionTime;
    public override int Capacity => capacity;
    public override int OutPutResourceAmount => _output;

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
        if (InternalStorage.Value >= Capacity)
        {
            ProductionProgress.Value = 1f;
        }
        isProducingFlag = false;
    }

    public override void CollectResources()
    {
        StorageManager.WheatCount.Value += InternalStorage.Value;
        InternalStorage.Value = 0;
    }

    public override void EnqueueProductionOrder() { }
    public override void DequeueProductionOrder() { }
}