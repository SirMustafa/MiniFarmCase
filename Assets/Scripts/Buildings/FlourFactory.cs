using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlourFactory : ResourceRequiringBuilding
{
    [Header("Building Stats")]
    [SerializeField] Transform _mill;
    [SerializeField] private Sprite _neededSprite;
    [SerializeField] private Sprite _mySprite;
    [SerializeField] private StorageManager.ResourceType _outputType;
    [SerializeField] private float _productionTime;
    [SerializeField] private int _outputResource;
    public override float ProductionTime => _productionTime;
    public override int OutputResourceAmount => _outputResource;
    public override StorageManager.ResourceType OutputResourceType => _outputType;
    public override Sprite NeededResourceSprite => _neededSprite;
    public override Sprite ProductionSprite => _mySprite;
    private void Start()
    {
        _mill.DORotate(new Vector3(0, 0, -360), 4f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }
}