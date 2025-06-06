using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadFactory : ResourceRequiringBuilding
{
    [Header("Building Stats")]
    [SerializeField] private float _productionTime;
    [SerializeField] private int _outputResource;
    [SerializeField] private Sprite _neededSprite;
    [SerializeField] private Sprite _mySprite;
    [SerializeField] private StorageManager.ResourceType _outputType;

    public override float ProductionTime => _productionTime;
    public override int OutputResourceAmount => _outputResource;
    public override StorageManager.ResourceType OutputResourceType => _outputType;
    public override Sprite NeededResourceSprite => _neededSprite;
    public override Sprite OutputResourcesSprite => _mySprite;

    private void Start()
    {
        GameManager.GameManagerInstance.AddBuildingToList(this);
    }
}