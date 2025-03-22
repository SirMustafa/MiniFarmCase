using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    private CancellationTokenSource cancelToken = new CancellationTokenSource();

    public override void CollectResources()
    {
        StorageManager.AddResource(OutputResourceType, InternalStorage.Value);
        InternalStorage.Value = 0;
    }

    public void EnqueueProductionOrder()
    {
        if (ProductionQueue.Value >= ProductionQueueCapacity 
            && StorageManager.GetResourceCount(_inputResourceType) < _inputCostPerOrder) return;

        StorageManager.RemoveResource(_inputResourceType, _inputCostPerOrder);
        ProductionQueue.Value++;

        if (!IsProducing)
        {
            ProduceResources().Forget();
        }
    }

    public void DequeueProductionOrder()
    {
        if (ProductionQueue.Value > 0)
        {
            ProductionQueue.Value--;
            StorageManager.AddResource(_inputResourceType, _inputCostPerOrder);

            if (ProductionQueue.Value is 0)
            {
                ProductionProgress.Value = 0f;
                cancelToken.Cancel();
            }
        }
    }

    public override async UniTask ProduceResources()
    {
        if (_isProducing) return;
        _isProducing = true;

        StartProductionTween();

        try
        {
            await ProcessProductionQueue(cancelToken.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Uretim iptal");
        }

        finally
        {
            PauseProductionTween();
            _isProducing = false;

            if (cancelToken.IsCancellationRequested)
            {
                cancelToken.Dispose();
                cancelToken = new CancellationTokenSource();
            }
        }
    }

    private async UniTask ProcessProductionQueue(CancellationToken token)
    {
        while (ProductionQueue.Value > 0)
        {
            await RunProductionCycle(token);
        }
    }

    private async UniTask RunProductionCycle(CancellationToken token)
    {
        float timer = ProductionProgress.Value * ProductionTime;

        while (timer < ProductionTime)
        {
            token.ThrowIfCancellationRequested();
            if (IsStorageFull)
            {
                await UniTask.WaitUntil(() => !IsStorageFull, cancellationToken: token);
            }
            ProductionProgress.Value = timer / ProductionTime;
            await UniTask.Yield();
            timer += Time.deltaTime;
        }

        ProductionProgress.Value = 0f;
        InternalStorage.Value += OutputResourceAmount;
        ProductionQueue.Value--;
    }

    public override void OfflineProduction(float offlineSeconds)
    {
        float productionTime = ProductionTime;
        int maxPossibleCycles = Mathf.FloorToInt(offlineSeconds / productionTime);
        int cyclesToProduce = Mathf.Min(maxPossibleCycles, ProductionQueue.Value);

        for (int i = 0; i < cyclesToProduce; i++)
        {
            if (InternalStorage.Value + OutputResourceAmount >= ProductionQueueCapacity)
            {
                InternalStorage.Value = ProductionQueueCapacity;
                ProductionProgress.Value = 0f;
                return;
            }

            InternalStorage.Value += OutputResourceAmount;
            ProductionQueue.Value--;
        }
    }
}