using System.Collections;
using System.Linq;
using UnityEngine;

public class Loader : Employee
{
    private SawmillFactory _targetFactory;

    private int _amountCargo;

    protected override int startUpgradeCost => 50;

    public int AmountCargo => _amountCargo;


    protected override void Start()
    {
        base.Start();

        _characterController = GetComponent<CharacterController>();
        Debug.Log(GameEntityRegistry.Instance.SawmillFactories.Count);

        if (GameEntityRegistry.Instance.SawmillFactories.Count > 0)
        {
            _targetFactory = GameEntityRegistry.Instance.SawmillFactories[Random.Range(0, GameEntityRegistry.Instance.SawmillFactories.Count)];
            _targetPosition = _targetFactory.transform.position;
        }
    }

    protected override void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Water"))
        {
            EmployeePools.Instance.LoaderPool.Release(gameObject);
        }
    }

    protected override void Move()
    {
        if (GameEntityRegistry.Instance.SawmillFactories.Count == 0 && _targetPosition == Vector3.zero) return;

        if(_targetFactory == null)
        {
            _targetFactory = GameEntityRegistry.Instance.SawmillFactories[Random.Range(0, GameEntityRegistry.Instance.SawmillFactories.Count)];

            if (_targetFactory != null)
            {
                _targetPosition = _targetFactory.transform.position;
            }
            else
            {
                _targetPosition = transform.position;
            }
        }

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

        if (_targetFactory.CurrentWood < LoaderSettings.basicLiftingCapacity * _level * _factory.Level)
        {
            _amountCargo = _targetFactory.CurrentWood;
            _targetFactory.CurrentWood = 0;
        }
        else
        {
            _amountCargo = LoaderSettings.basicLiftingCapacity * _level * _factory.Level;
            _targetFactory.CurrentWood -= LoaderSettings.basicLiftingCapacity * _level * _factory.Level;
        }

        _targetPosition = _startPosition;

        _isWaitingAtWaypoint = false;
    }

    protected override void DeliverResource()
    {
        ResourceManager.Instance.Wood += _amountCargo;

        _targetFactory = GameEntityRegistry.Instance.SawmillFactories[Random.Range(0, GameEntityRegistry.Instance.SawmillFactories.Count)];

        _targetPosition = _targetFactory.transform.position;
    }

    public override void Improve()
    {
        base.Improve();
        Level++;
    }

    public override void DeleteEntity()
    {
        base.DeleteEntity();

        EmployeePools.Instance.LoaderPool.Release(gameObject);

        if (GameEntityRegistry.Instance.SawmillFactories.Count > 0)
        {
            _targetFactory = GameEntityRegistry.Instance.SawmillFactories[Random.Range(0, GameEntityRegistry.Instance.SawmillFactories.Count)];
            _targetPosition = _targetFactory.transform.position;
        }
    }
}
