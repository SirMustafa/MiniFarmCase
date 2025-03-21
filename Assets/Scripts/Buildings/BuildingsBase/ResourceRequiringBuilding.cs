using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceRequiringBuilding : BuildingsBase
{
    [Header("Production Stats")]
    [SerializeField] private StorageManager.ResourceType _inputResourceType;
    [SerializeField] private int _inputCostPerOrder;
    [SerializeField] private int _maxProductionQueue;

    public StorageManager.ResourceType InputResourceType => _inputResourceType;
    public override bool IsProducing => _isProducing;
    public override int ProductionQueueCapacity => _maxProductionQueue;   
    public abstract Sprite NeededResourceSprite { get; }
    public abstract override float ProductionTime { get; }
    public abstract override int OutputResourceAmount { get; }
    public abstract override StorageManager.ResourceType OutputResourceType { get; }

    public int InputCostPerOrder => _inputCostPerOrder;
    private bool _isProducing = false;

    public override void CollectResources()
    {
        isStorageFull = false;
        StorageManager.AddResource(OutputResourceType, InternalStorage.Value);
        InternalStorage.Value = 0;
        isStorageFull = false;
    }

    public void EnqueueProductionOrder()
    {
        if (ProductionQueue.Value >= ProductionQueueCapacity) return;

        if (StorageManager.GetResourceCount(_inputResourceType) < _inputCostPerOrder) return;

        StorageManager.RemoveResource(_inputResourceType, _inputCostPerOrder);
        ProductionQueue.Value++;

        if (!IsProducing) ProduceResources().Forget();
    }

    public void DequeueProductionOrder()
    {
        if (ProductionQueue.Value > 0)
        {
            ProductionQueue.Value--;
            StorageManager.AddResource(_inputResourceType, _inputCostPerOrder);
        }     
    }

    public override async UniTask ProduceResources()
    {
        if (IsProducing) return;

        _isProducing = true;

        StartProductionTween();

        await ProcessProductionQueue();

        PauseProductionTween();
        _isProducing = false;
    }
}