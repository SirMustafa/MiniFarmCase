using Cysharp.Threading.Tasks;
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

    private bool isProducing = false;

    private void Start()
    {
        mill.DORotate(new Vector3(0, 0, -360), 4f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    protected override void ProduceResources()
    {
        ProcessProductionQueue().Forget();
    }

    public override void EnqueueProductionOrder()
    {
        if (ProductionQueue.Value >= maxProductionQueue || InternalStorage.Value >= Capacity)
            return;
        if (Storage.WheatCount.Value < flourCostPerOrder)
            return;
        ProductionQueue.Value++;
        if (!isProducing)
            ProduceResources();
    }

    public override void CancelProductionOrder()
    {
        if (ProductionQueue.Value > 0)
            ProductionQueue.Value--;
        else
            Debug.Log("Ýptal edilebilecek üretim emri yok!");
    }

    private async UniTaskVoid ProcessProductionQueue()
    {
        isProducing = true;
        while (ProductionQueue.Value > 0)
        {
            if (Storage.WheatCount.Value < flourCostPerOrder)
                break;
            Storage.WheatCount.Value -= flourCostPerOrder;
            float timer = 0f;
            while (timer < productionTime)
            {
                ProductionProgress.Value = timer / productionTime;
                await UniTask.Yield();
                timer += Time.deltaTime;
            }
            ProductionProgress.Value = 0f;
            Storage.FlourCount.Value++;
            InternalStorage.Value++;
            ProductionQueue.Value--;
        }
        isProducing = false;
    }

    public override void OnClick()
    {
        if (!isPanelOpened)
        {
            base.OnClick();
            isPanelOpened = true;
        }
        else
        {
            CollectResources();

            if (!isProducing)
            {
                ProduceResources();
            }
        }
    }

    public void CollectResources()
    {
        Storage.FlourCount.Value += InternalStorage.Value;
        InternalStorage.Value = 0;
    }
}