using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tent : GameEntity, IBuildable
{
    protected override int startUpgradeCost => 30;

    private int _startFoodCount = 10;

    public int BuildMoneyCost { get; set; } = 100;
    public int BuildWoodCost { get; set; } = 3000;
    public bool IsStartBuilding { get; set; } = false;


    protected override void Start()
    {
        base.Start();

        _currentUpgradeCost = startUpgradeCost;
    }

    public override void Improve()
    {
        base.Improve();
        ResourceManager.Instance.Food += _startFoodCount * _level;
        Level++;
    }
}
