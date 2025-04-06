using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseFactory : AbstractFactory, IBuildable
{
    protected override int startUpgradeCost => 100;
    protected override int startCreateCost => 50;

    public int BuildMoneyCost { get; set; } = 150;
    public int BuildWoodCost { get; set; } = 8000;

    protected override void Start()
    {
        base.Start();
    }

    public override void CreateEmployee()
    {
        base.CreateEmployee();

        Loader loader = EmployeePools.Instance.LoaderPool.Get(_spawnPosition.position).GetComponent<Loader>();
        loader.SetStartPosition(_spawnPosition.position);
        loader.SetFactory(this);

        _employees.Add(loader);
    }

    public override void Improve()
    {
        base.Improve();
        Level++;
    }
}
