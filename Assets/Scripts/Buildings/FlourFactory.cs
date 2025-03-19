using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class FlourFactory : BuildingsBase, INeedResource
{
    [Header("BuildStats")]
    [SerializeField] private float _productionTime;
    [SerializeField] private int _sourceCostPerOrder;
    [SerializeField] private int _maxProductionQueue;
    [SerializeField] private int _outputResource;
    [SerializeField] private Sprite _neededResource;
    [SerializeField] private Transform _mill;

    private bool _isProducingFlag = false;
    public ReactiveProperty<int> ProductionQueue { get; } = new ReactiveProperty<int>(0);

    public override float ProductionTime => _productionTime;
    public override int Capacity => _maxProductionQueue;
    public override bool IsProducing => _isProducingFlag;
    public override int OutPutResourceAmount => _outputResource;

    public Sprite NeededResourceSprite() => _neededResource;
    public int InputResource() => _sourceCostPerOrder;
    public int OutputResource() => _outputResource;

    private void Start()
    {
        _mill.DORotate(new Vector3(0, 0, -360), 4f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }

    public override void EnqueueProductionOrder()
    {
        if (ProductionQueue.Value >= _maxProductionQueue || InternalStorage.Value >= Capacity)
            return;

        if (StorageManager.WheatCount.Value < _sourceCostPerOrder)
            return;

        ProductionQueue.Value++;
        if (!_isProducingFlag)
            ProduceResources();
    }

    public override void DequeueProductionOrder()
    {
        if (ProductionQueue.Value > 0)
            ProductionQueue.Value--;
        else
            Debug.Log("No production order to cancel");
    }

    protected override async void ProduceResources()
    {
        if (_isProducingFlag)
            return;

        _isProducingFlag = true;
        await ProcessProductionQueue(ProductionQueue, StorageManager.WheatCount, _sourceCostPerOrder);

        if (InternalStorage.Value >= Capacity)
            ProductionProgress.Value = 1f;

        _isProducingFlag = false;
    }

    public override void CollectResources()
    {
        StorageManager.FlourCount.Value += InternalStorage.Value;
        InternalStorage.Value = 0;
    }
}