using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BreadFactory : BuildingsBase
{
    [SerializeField] private float productionTime = 40f;
    [SerializeField] private int breadCostPerOrder = 1;
    [SerializeField] private int maxProductionQueue = 10;
    public ReactiveProperty<int> ProductionQueue { get; private set; } = new ReactiveProperty<int>(0);

    public override float ProductionTime => productionTime;
    public override int Capacity => maxProductionQueue;

    private bool isProducingFlag = false;
    public override bool IsProducing => isProducingFlag;

    public override void EnqueueProductionOrder()
    {
        if (ProductionQueue.Value >= maxProductionQueue || InternalStorage.Value >= Capacity)
            return;

        if (Storage.FlourCount.Value < breadCostPerOrder)
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
        await ProcessProductionQueue(ProductionQueue, Storage.FlourCount, breadCostPerOrder, Storage.BreadCount);
        isProducingFlag = false;
    }

    public override void CollectResources()
    {
        Storage.BreadCount.Value += InternalStorage.Value;
        InternalStorage.Value = 0;
    }
}