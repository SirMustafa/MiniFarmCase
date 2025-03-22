using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceFreeBuilding : BuildingsBase
{
    public override bool IsProducing => _isProducing;
    public override abstract int ProductionQueueCapacity { get; }
    public override abstract Sprite OutputResourcesSprite { get; }
    public abstract override float ProductionTime { get; }
    public abstract override int OutputResourceAmount { get; }
    public abstract override StorageManager.ResourceType OutputResourceType { get; }

    private bool _isProducing = false;

    private void Start()
    {
        GameManager.GameManagerInstance.AddBuildingToList(this);
        ProduceResources().Forget();
    }

    public override async UniTask ProduceResources()
    {
        if (_isProducing) return;
        _isProducing = true;

        StartProductionTween();

        while (!IsStorageFull)
        {
            float timer = 0f;
            while (timer < ProductionTime)
            {
                ProductionProgress.Value = timer / ProductionTime;
                await UniTask.Yield();
                timer += Time.deltaTime;

                if (IsStorageFull)
                {
                    PauseProductionTween();
                    _isProducing = false;
                    return;
                }
            }

            ProductionProgress.Value = 0f;
            InternalStorage.Value += OutputResourceAmount;
        }

        PauseProductionTween();
        _isProducing = false;
    }

    public override void OfflineProduction(float offlineSeconds)
    {
        float productionTime = ProductionTime;
        int maxPossibleCycles = Mathf.FloorToInt(offlineSeconds / productionTime);

        for (int i = 0; i < maxPossibleCycles && !IsStorageFull; i++)
        {
            if (InternalStorage.Value + OutputResourceAmount >= ProductionQueueCapacity)
            {
                InternalStorage.Value = ProductionQueueCapacity;
                ProductionProgress.Value = 0f;
                return;
            }

            InternalStorage.Value += OutputResourceAmount;
        }
    }

    public override void CollectResources()
    {
        StorageManager.AddResource(OutputResourceType, InternalStorage.Value);
        InternalStorage.Value = 0;

        _isProducing = false;
        if (!IsProducing)
        {
            ProduceResources().Forget();
        }
    }
}