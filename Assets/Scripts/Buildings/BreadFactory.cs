using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BreadFactory : BuildingsBase, INeedResource
{
    [Header("BuildStats")]
    [SerializeField] private float _productionTime;
    [SerializeField] private int _sourceCostPerOrder;
    [SerializeField] private int _maxProductionQueue;
    [SerializeField] private int _outputResource;
    [SerializeField] private Sprite _neededResource;
    private bool _isProducingFlag = false;

    public ReactiveProperty<int> ProductionQueue { get; private set; } = new ReactiveProperty<int>(0);
    public override float ProductionTime => _productionTime;
    public override int Capacity => _maxProductionQueue;
    public override bool IsProducing => _isProducingFlag;
    public override int OutPutResourceAmount => _outputResource;

    public Sprite NeededResourceSprite() => _neededResource;

    public int InputResource() => _sourceCostPerOrder;

    public int OutputResource() => _outputResource;

    public override void EnqueueProductionOrder()
    {
        if (ProductionQueue.Value >= _maxProductionQueue || InternalStorage.Value >= Capacity)
            return;

        if (StorageManager.FlourCount.Value < _sourceCostPerOrder)
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
            Debug.Log("Ýptal edilebilecek üretim emri yok!");
    }

    protected override async void ProduceResources()
    {
        if (_isProducingFlag)
            return;

        _isProducingFlag = true;
        await ProcessProductionQueue(ProductionQueue, StorageManager.FlourCount, _sourceCostPerOrder);

        if (InternalStorage.Value >= Capacity)
            ProductionProgress.Value = 1f;

        _isProducingFlag = false;
    }

    public override void CollectResources()
    {
        StorageManager.BreadCount.Value += InternalStorage.Value;
        InternalStorage.Value = 0;
    }
}