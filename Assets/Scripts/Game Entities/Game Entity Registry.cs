using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class GameEntityRegistry : MonoBehaviour
{
    public static GameEntityRegistry Instance { get; private set; }

    [Header("General")]
    [SerializeField] private PlacementSystem _placementSystem;

    private List<GameEntity> _entities = new();

    public List<GameEntity> Entities => _entities;

    private List<Employee> _employees = new();

    public List<Employee> Employees => _employees;

    [Header("Trees")]
    [SerializeField] private Transform[] _trees;

    [Header("Dealer")]
    [SerializeField] private Transform[] _intermediateWaypoints;
    [SerializeField] private Transform _finishWaypoint;

    public Transform[] Trees => _trees;
    public Transform[] IntermediateWaypoints => _intermediateWaypoints;
    public Transform FinishWaypoint => _finishWaypoint;
    public List<SawmillFactory> SawmillFactories { get; set; } = new();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EntityRegister(GameEntity entity)
    {
        if (!_entities.Contains(entity))
        {
            _entities.Add(entity);
        }
    }

    public void EmployeeRegister(Employee employee)
    {
        _employees.Add(employee);
    }

    public void EntityUnregister(GameEntity entity)
    {
        if (_entities.Contains(entity))
        {
            _entities.Remove(entity);
        }
    }

    public void EmployeeUnregister(Employee employee)
    {
        _employees.Remove(employee);
    }
}
