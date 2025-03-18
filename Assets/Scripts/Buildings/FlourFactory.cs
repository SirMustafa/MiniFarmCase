using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class FlourFactory : BuildingsBase
{
    [SerializeField] private Transform mill;
    [SerializeField] private float productionTime = 40f;
    [SerializeField] private int flourCostPerOrder = 1;
    [SerializeField] private int maxProductionQueue = 10;
    public ReactiveProperty<int> ProductionQueue { get; private set; } = new ReactiveProperty<int>(0);

    public override float ProductionTime => productionTime;
    public override int Capacity => maxProductionQueue;

    private bool isProducingFlag = false;
    public override bool IsProducing => isProducingFlag;

    private void Start()
    {
        mill.DORotate(new Vector3(0, 0, -360), 4f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    public override void EnqueueProductionOrder()
    {
        if (ProductionQueue.Value >= maxProductionQueue || InternalStorage.Value >= Capacity)
            return;

        if (Storage.WheatCount.Value < flourCostPerOrder)
            return;
        ProductionQueue.Value++;
        if (!isProducingFlag)
            ProduceResources();
    }

    public override void CancelProductionOrder()
    {
        if (ProductionQueue.Value > 0)
            ProductionQueue.Value--;
        else
            Debug.Log("Ýptal edilebilecek üretim emri yok!");
    }

    protected override async void ProduceResources()
    {
        if (isProducingFlag) return;
        isProducingFlag = true;
        await ProcessProductionQueue(ProductionQueue, Storage.WheatCount, flourCostPerOrder, Storage.FlourCount);
        isProducingFlag = false;
    }

    public override void CollectResources()
    {
        Storage.FlourCount.Value += InternalStorage.Value;
        InternalStorage.Value = 0;
    }
}