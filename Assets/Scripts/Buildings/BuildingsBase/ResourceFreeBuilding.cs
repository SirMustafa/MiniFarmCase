using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceFreeBuilding : BuildingsBase
{
    public override bool IsProducing => _isProducing;
    public override abstract int ProductionQueueCapacity { get; }
    public override abstract Sprite ProductionSprite { get; }

    public abstract override float ProductionTime { get; }
    public abstract override int OutputResourceAmount { get; }
    public abstract override StorageManager.ResourceType OutputResourceType { get; }
    
    private bool _isProducing = false;

    protected override async void ProduceResources()
    {
        if (_isProducing) return;
        _isProducing = true;

        StartProductionTween();

        while (InternalStorage.Value < ProductionQueueCapacity)
        {
            float timer = 0f;
            while (timer < ProductionTime)
            {
                ProductionProgress.Value = timer / ProductionTime;
                await UniTask.Yield();
                timer += Time.deltaTime;
            }
            ProductionProgress.Value = 0f;
            InternalStorage.Value += OutputResourceAmount;
        }

        PauseProductionTween();
        _isProducing = false;
    }

    public override void CollectResources()
    {
        StorageManager.AddResource(OutputResourceType, InternalStorage.Value);
        InternalStorage.Value = 0;

        if (!IsProducing)
            ProduceResources();
    }

    private void Start()
    {
        ProduceResources();
    }
}