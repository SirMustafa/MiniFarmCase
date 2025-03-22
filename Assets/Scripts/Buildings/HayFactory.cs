using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HayFactory : ResourceFreeBuilding
{
    [Header("Building Stats")]
    [SerializeField] private float _productionTime;
    [SerializeField] private int _outputResource;
    [SerializeField] private int _capacity;
    [SerializeField] private Sprite _mySprite;
    [SerializeField] private StorageManager.ResourceType _outputType;

    public override float ProductionTime => _productionTime;
    public override int OutputResourceAmount => _outputResource;
    public override StorageManager.ResourceType OutputResourceType => _outputType;
    public override Sprite OutputResourcesSprite => _mySprite;
    public override int ProductionQueueCapacity => _capacity;
}