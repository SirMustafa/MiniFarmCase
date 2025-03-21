using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class BuildingsBase : MonoBehaviour, IClickable
{
    public ReactiveProperty<int> InternalStorage { get; } = new ReactiveProperty<int>(0);
    public ReactiveProperty<float> ProductionProgress { get; } = new ReactiveProperty<float>(0f);
    public ReactiveProperty<int> ProductionQueue { get; protected set; } = new ReactiveProperty<int>(0);

    public abstract StorageManager.ResourceType OutputResourceType { get; }
    public abstract Sprite outputResourcesSprite { get; }
    public abstract bool IsProducing { get; }
    public abstract float ProductionTime { get; }
    public abstract int OutputResourceAmount { get; }
    public abstract int ProductionQueueCapacity { get; }

    public bool IsPanelOpened { get; set; }
    protected Tween productionTween;
    private float tweenDuration = 1f;
    private float _transformsY;

    protected abstract UniTask ProduceResources();
    public abstract void CollectResources();

    private void Awake()
    {
        _transformsY = transform.localScale.y;
    }
    private void Start()
    {
        GameManager.GameManagerInstance.AddBuildingToList(this);
    }
    public virtual void OnClick()
    {
        if (!IsPanelOpened)
        {
            UiManager.UiManagerInstance.ShowBuildingPanel(this);
            IsPanelOpened = true;
        }
        else
        {
            CollectResources();
        }
    }

    protected void StartProductionTween()
    {
        float targetY = _transformsY * 1.1f;
        
        if (productionTween is null)
        {
            productionTween = transform.DOScaleY(targetY, tweenDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).Pause();
        }
        productionTween.Restart();
    }

    protected void PauseProductionTween()
    {
        productionTween.Pause();
    }

    protected async UniTask ProcessProductionQueue()
    {
        while (ProductionQueue.Value > 0)
        {
            float timer = ProductionProgress.Value * ProductionTime;
            while (timer < ProductionTime)
            {
                ProductionProgress.Value = timer / ProductionTime;
                await UniTask.DelayFrame(1);
                timer += Time.deltaTime;
            }
            ProductionProgress.Value = 0f;
            InternalStorage.Value += OutputResourceAmount;

            if (ProductionQueue.Value is 0) return;
            ProductionQueue.Value--;
        }
    }
    public void OfflineProduction(float offlineSeconds)
    {
        float productionTime = ProductionTime;

        while (offlineSeconds >= productionTime && ProductionQueue.Value > 0)
        {
            if (InternalStorage.Value + OutputResourceAmount >= ProductionQueueCapacity)
            {
                InternalStorage.Value = ProductionQueueCapacity;
                ProductionQueue.Value = 0;
                return;
            }

            offlineSeconds -= productionTime;
            InternalStorage.Value += OutputResourceAmount;
            ProductionQueue.Value--;
        }

        if (ProductionQueue.Value > 0)
        {
            ProductionProgress.Value = offlineSeconds / productionTime;
        }
    }
}