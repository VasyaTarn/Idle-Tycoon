using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Employee : GameEntity, IDeletable
{
    [SerializeField] protected EmployeeSettings _settings;
    protected Vector3 _startPosition;

    protected CharacterController _characterController;

    protected Vector3 _targetDirection;
    protected Vector3 _targetPosition;

    protected bool _isWaitingAtWaypoint = false;

    [SerializeField] protected float _waitTimeAtWaypoint;

    [Space(20)]
    [Header("Save")]
    [SerializeField] private string _prefabId;
    protected override bool ShouldRegister => false;

    protected AbstractFactory _factory;

    public AbstractFactory Factory => _factory;

    public string PrefabId => _prefabId;



    protected override void Start()
    {
        base.Start();

        if (!_dataLoaded)
        {
            _currentUpgradeCost = startUpgradeCost;
        }
    }

    protected abstract void OnControllerColliderHit(ControllerColliderHit hit);

    void Update()
    {
        Move();
    }

    protected abstract void Move();

    protected abstract IEnumerator WaitForAnimationAndMoveToNextWaypoint();

    protected abstract void DeliverResource();

    public void SetStartPosition(Vector3 position)
    {
        _startPosition = position;
    }

    public void SetFactory(AbstractFactory factory)
    {
        _factory = factory;
    }

    public virtual void DeleteEntity()
    {
        ResourceManager.Instance.Food += _factory.FoodCost;
        _factory.Employees.Remove(this);
        Level = 1;
        _currentUpgradeCost = startUpgradeCost;
    }

    public override GameEntitySaveData SaveData()
    {
        GameEntitySaveData data = base.SaveData();

        return data;
    }

    public override void LoadData(GameEntitySaveData data)
    {
        base.LoadData(data);

        _dataLoaded = true;
    }
}
