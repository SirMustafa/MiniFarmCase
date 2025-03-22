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
    public abstract Sprite OutputResourcesSprite { get; }
    public abstract bool IsProducing { get; }
    public abstract float ProductionTime { get; }
    public abstract int OutputResourceAmount { get; }
    public abstract int ProductionQueueCapacity { get; }

    protected virtual bool IsStorageFull => InternalStorage.Value >= ProductionQueueCapacity;

    public bool IsPanelOpened { get; set; }

    protected Tween productionTween;
    private float _tweenDuration = 1f;
    private float _originalScaleY;

    public abstract UniTask ProduceResources();
    public abstract void CollectResources();
    public abstract void OfflineProduction(float offlineSeconds);

    private void Awake()
    {
        _originalScaleY = transform.localScale.y;
    }

    public void OnClick()
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
        float targetY = _originalScaleY * 1.1f;

        if (productionTween is null)
        {
            productionTween = transform.DOScaleY(targetY, _tweenDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).Pause();
        }

        productionTween.Restart();
    }

    protected void PauseProductionTween()
    {
        productionTween.Pause();
    }
}