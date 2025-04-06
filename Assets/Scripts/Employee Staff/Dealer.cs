using System.Collections;
using System.Linq;
using UnityEngine;

public class Dealer : Employee
{
    private int _currentWaypointIndex = 0;
    private bool _movingToFinish = false;
    private bool _returningBack = false;
    private bool _finishedReturn = false;

    protected override int startUpgradeCost => 60;


    protected override void Start()
    {
        base.Start();
        _characterController = GetComponent<CharacterController>();

        if (GameEntityRegistry.Instance.IntermediateWaypoints.Length > 0)
        {
            _targetPosition = GameEntityRegistry.Instance.IntermediateWaypoints[0].position;
        }
        else
        {
            _targetPosition = GameEntityRegistry.Instance.FinishWaypoint.position;
            _movingToFinish = true;
        }
    }

    protected override void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Water"))
        {
            EmployeePools.Instance.DealerPool.Release(gameObject);
        }
    }

    protected override void Move()
    {
        if (_targetPosition == Vector3.zero) return;

        if (Inventory.Instance != null)
        {
            bool hasItemsToDeliver = Inventory.Instance.Items.Any(item => item.Value.Count > 0);

            if (!hasItemsToDeliver && !_returningBack && !_finishedReturn)
                return;

            if (!_returningBack && !_movingToFinish)
            {
                if (!Ship.Instance.IsShopPoint || !hasItemsToDeliver)
                    return;
            }

            Vector3 horizontalPosition = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 horizontalTarget = new Vector3(_targetPosition.x, 0, _targetPosition.z);
            float distanceToTarget = Vector3.Distance(horizontalPosition, horizontalTarget);

            if (distanceToTarget < _settings.TargetThreshold && !_isWaitingAtWaypoint)
            {
                StartCoroutine(WaitForAnimationAndMoveToNextWaypoint());
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
    }

    protected override IEnumerator WaitForAnimationAndMoveToNextWaypoint()
    {
        _isWaitingAtWaypoint = true;

        yield return new WaitForSeconds(Random.Range(0.5f, _waitTimeAtWaypoint + 1));

        if (!_movingToFinish && !_returningBack)
        {
            if (_currentWaypointIndex < GameEntityRegistry.Instance.IntermediateWaypoints.Length - 1)
            {
                _currentWaypointIndex++;
                _targetPosition = GameEntityRegistry.Instance.IntermediateWaypoints[_currentWaypointIndex].position;
            }
            else
            {
                _movingToFinish = true;
                _targetPosition = GameEntityRegistry.Instance.FinishWaypoint.position;
            }
        }
        else if (_movingToFinish)
        {

            _returningBack = true;
            _movingToFinish = false;
            _currentWaypointIndex = GameEntityRegistry.Instance.IntermediateWaypoints.Length - 1;
            _targetPosition = GameEntityRegistry.Instance.IntermediateWaypoints[_currentWaypointIndex].position;
        }
        else if (_returningBack && !_finishedReturn)
        {
            if (_currentWaypointIndex > 0)
            {
                _currentWaypointIndex--;
                _targetPosition = GameEntityRegistry.Instance.IntermediateWaypoints[_currentWaypointIndex].position;
            }
            else
            {
                _finishedReturn = true;
                _targetPosition = _startPosition;
            }
        }
        else if (_finishedReturn)
        {
            DeliverResource();

            _currentWaypointIndex = 0;
            _movingToFinish = false;
            _returningBack = false;
            _finishedReturn = false;

            _targetPosition = GameEntityRegistry.Instance.IntermediateWaypoints.Length > 0 
                ? GameEntityRegistry.Instance.IntermediateWaypoints[0].position 
                : GameEntityRegistry.Instance.FinishWaypoint.position;
        }

        _isWaitingAtWaypoint = false;
    }

    protected override void DeliverResource()
    {
        var availableItems = Inventory.Instance.Items.Values
            .Where(item => item.Count > 0)
            .ToList();

        if (availableItems.Count != 0)
        {
            ItemBlock item = Inventory.Instance.Items[Random.Range(0, availableItems.Count)];

            Inventory.Instance.Items[item.ItemID].Count--;
            ResourceManager.Instance.Money += Inventory.Instance.Items[item.ItemID].SaleCost;
        }
    }

    public override void DeleteEntity()
    {
        base.DeleteEntity();

        EmployeePools.Instance.DealerPool.Release(gameObject);

        if (GameEntityRegistry.Instance.IntermediateWaypoints.Length > 0)
        {
            _targetPosition = GameEntityRegistry.Instance.IntermediateWaypoints[0].position;
        }
        else
        {
            _targetPosition = GameEntityRegistry.Instance.FinishWaypoint.position;
            _movingToFinish = true;
        }
    }

    public override void Improve()
    {
        base.Improve();
        Level++;
    }
}
