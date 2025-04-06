using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public static Ship Instance { get; private set; }

    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private Transform _shopPoint;
    [SerializeField] private float _speed; 
    [SerializeField] private float _stopTime;
    [SerializeField] private float _delay;

    private bool _isShopPoint;

    public bool IsShopPoint => _isShopPoint;

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

    private void Start()
    {
        StartCoroutine(MoveShip());
    }

    private IEnumerator MoveShip()
    {
        transform.position = _startPoint.position; 

        yield return MoveToPosition(_shopPoint.position); 
        yield return new WaitForSeconds(_stopTime);
        _isShopPoint = false;

        yield return MoveToPosition(_endPoint.position); 
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, _speed * Time.deltaTime);
            yield return null; 
        }

        if(transform.position == _shopPoint.position)
        {
            _isShopPoint = true;
        }

        if (transform.position == _endPoint.position)
        {
            transform.position = _startPoint.position;

            yield return new WaitForSeconds(_delay);

            yield return MoveShip();
        }

    }

}
