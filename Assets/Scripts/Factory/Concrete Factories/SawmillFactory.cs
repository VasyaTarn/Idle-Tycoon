using System;
using TMPro;
using UnityEngine;

public class SawmillFactory : AbstractFactory, IBuildable
{
    [Header("UI")]
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TMP_Text _currentWoodText;
    [SerializeField] private TMP_Text _maxWoodText;

    [Header("Sawmill")]
    [SerializeField] private int _startWoodCount;

    private int _currentWood;
    private int _maxWood;

    protected override int startUpgradeCost => 100;
    protected override int startCreateCost => 50;

    public int BuildMoneyCost { get; set; } = 200;
    public int BuildWoodCost { get; set; } = 5000;

    public int CurrentWood
    {
        get => _currentWood;
        set
        {
            _currentWood = value;
            OnCurrentWoodChanged(value);
        }
    }

    public int MaxWood
    {
        get => _maxWood;
        set
        {
            _maxWood = value;
            OnMaxWoodChanged(value);
        }
    }

    protected override void Start()
    {
        base.Start();

        if(!_dataLoaded)
        {
            MaxWood = _startWoodCount;
        }

        _canvas.worldCamera = Camera.main;
    }

    public override void CreateEmployee()
    {
        base.CreateEmployee();

        Woodcutter woodcutter = EmployeePools.Instance.WoodcutterPool.Get(_spawnPosition.position).GetComponent<Woodcutter>();
        woodcutter.SetStartPosition(_spawnPosition.position);
        woodcutter.SetFactory(this);

        _employees.Add(woodcutter);
    }

    public override void Improve()
    {
        MaxWood *= 2;

        base.Improve();
        Level++;
    }

    public void UpdateCurrentWoodText(int number)
    {
        _currentWoodText.text = ResourceFormatter.FormatResourceNumber(number);
    }

    public void UpdateMaxWoodText(int number)
    {
        _maxWoodText.text = ResourceFormatter.FormatResourceNumber(number);
    }

    private void OnCurrentWoodChanged(int number)
    {
        UpdateCurrentWoodText(number);
    }

    private void OnMaxWoodChanged(int number)
    {
        UpdateMaxWoodText(number);
    }

    public void CollectWood(int number)
    {
        if (CurrentWood < number)
        {
            CurrentWood = 0;
            ResourceManager.Instance.Wood += CurrentWood;
        }
        else
        {
            CurrentWood -= number;
            ResourceManager.Instance.Wood += number;
        }
    }

    public override GameEntitySaveData SaveData()
    {
        GameEntitySaveData data = base.SaveData();

        data.currentWood = CurrentWood;
        data.maxWood = MaxWood;

        return data;
    }

    public override void LoadData(GameEntitySaveData data)
    {
        base.LoadData(data);

        CurrentWood = data.currentWood;
        MaxWood = data.maxWood;

        _dataLoaded = true;
    }
}
