using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreeEntity : GameEntity, IModelUpgradeable
{
    [Header("Tree UI")]
    [SerializeField] private Canvas _cutCanvas;
    public TreeEntity NextLevelObject { get; set; }
    protected override int startUpgradeCost => 200;

    protected override void Start()
    {
        base.Start();

        _currentUpgradeCost = startUpgradeCost;

        _cutCanvas.worldCamera = Camera.main;
    }

    public override void Improve()
    {
        base.Improve();
        Level++;
    }

    public void CutDown(int wood)
    {
        ResourceManager.Instance.Wood += wood;
    }
}
