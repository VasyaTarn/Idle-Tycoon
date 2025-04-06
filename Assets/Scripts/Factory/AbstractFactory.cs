using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractFactory : GameEntity, IDeletable
{
    [Space(20)]
    [SerializeField] protected Transform _spawnPosition;
    [SerializeField] protected Employee _employeePrefab;
    protected List<Employee> _employees = new();

    protected int _currentCreateCost;
    protected int _createMultiplier = 2;

    [SerializeField] private int _foodCost;


    protected abstract int startCreateCost { get; }

    public int CurrentCreateCost => _currentCreateCost;
    public int CreateMultiplier => _createMultiplier;
    public int FoodCost => _foodCost;
    public List<Employee> Employees => _employees;

    protected override void Start()
    {
        base.Start();

        if (!_dataLoaded)
        {
            _isFactory = true;
        }

        _currentCreateCost = startCreateCost;
    }

    public virtual void CreateEmployee()
    {
        ResourceManager.Instance.Money -= startCreateCost * _createMultiplier * Level;
        ResourceManager.Instance.Food -= _foodCost;
    }

    public void DeleteEntity()
    {
        foreach (Employee employee in new List<Employee>(_employees))
        {
            employee.DeleteEntity();
        }

        PlacementSystem.Instance.PlacedObjects.Remove(this);
        Destroy(gameObject);
    }

    public override GameEntitySaveData SaveData()
    {
        return base.SaveData();
    }

    public override void LoadData(GameEntitySaveData data)
    {
        base.LoadData(data);

        _dataLoaded = true;
    }
}
