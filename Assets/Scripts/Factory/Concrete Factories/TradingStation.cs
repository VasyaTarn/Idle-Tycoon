using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingStation : AbstractFactory, IBuildable
{
    protected override int startUpgradeCost => 120;
    protected override int startCreateCost => 60;

    public int BuildMoneyCost { get; set; } = 250;
    public int BuildWoodCost { get; set; } = 4000;

    protected override void Start()
    {
        base.Start();
    }

    public override void CreateEmployee()
    {
        base.CreateEmployee();

        Dealer dealer = EmployeePools.Instance.DealerPool.Get(_spawnPosition.position).GetComponent<Dealer>();
        dealer.SetStartPosition(_spawnPosition.position);
        dealer.SetFactory(this);

        _employees.Add(dealer);
    }

    public override void Improve()
    {
        base.Improve();
        Level++;
    }
}
