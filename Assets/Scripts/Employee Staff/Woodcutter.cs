using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Woodcutter : Employee
{
    private TreeEntity _targetTree;

    protected override int startUpgradeCost => 50;


    protected override void Start()
    {
        base.Start();

        _characterController = GetComponent<CharacterController>();

        _targetTree = GameEntityRegistry.Instance.Trees[Random.Range(0, GameEntityRegistry.Instance.Trees.Length)].GetComponent<TreeEntity>();

        _targetPosition = _targetTree.transform.position;
    }

    protected override void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Water"))
        {
            EmployeePools.Instance.WoodcutterPool.Release(gameObject);
        }
    }
    protected override void Move()
    {
        if (GameEntityRegistry.Instance.Trees.Length == 0 && _targetPosition == Vector3.zero) return;

        Vector3 horizontalPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 horizontalTarget = new Vector3(_targetPosition.x, 0, _targetPosition.z);
        float distanceToTarget = Vector3.Distance(horizontalPosition, horizontalTarget);

        if (distanceToTarget < _settings.TargetThreshold && !_isWaitingAtWaypoint)
        {
            if (_targetPosition == _startPosition)
            {
                DeliverResource();
            }
            else
            {
                StartCoroutine(WaitForAnimationAndMoveToNextWaypoint());
            }
        }

        _targetDirection = (_targetPosition - transform.position).normalized;
        _targetDirection.y = 0;

        if (_targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _settings.RotationSpeed * Time.deltaTime);
        }

        Vector3 moveDirection = transform.forward * _settings.MoveSpeed;
        moveDirection.y = Physics.gravity.y;

        if (!_isWaitingAtWaypoint)
        {
            _characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    protected override IEnumerator WaitForAnimationAndMoveToNextWaypoint()
    {
        _isWaitingAtWaypoint = true;

        yield return new WaitForSeconds(_waitTimeAtWaypoint);

        _targetPosition = _startPosition;

        _isWaitingAtWaypoint = false;
    }

    protected override void DeliverResource()
    {
        SawmillFactory sawmill = (SawmillFactory)_factory;

        if (sawmill.CurrentWood <= sawmill.MaxWood)
        {
            sawmill.CurrentWood += WoodcutterSettings.basicWoodProduction * _level * _targetTree.Level;
        }

        _targetTree = GameEntityRegistry.Instance.Trees[Random.Range(0, GameEntityRegistry.Instance.Trees.Length)].GetComponent<TreeEntity>();

        _targetPosition = _targetTree.transform.position;
    }

    public override void Improve()
    {
        base.Improve();
        Level++;
    }

    public override void DeleteEntity()
    {
        base.DeleteEntity();

        EmployeePools.Instance.WoodcutterPool.Release(gameObject);

        _targetTree = GameEntityRegistry.Instance.Trees[Random.Range(0, GameEntityRegistry.Instance.Trees.Length)].GetComponent<TreeEntity>();

        _targetPosition = _targetTree.transform.position;
    }
}
