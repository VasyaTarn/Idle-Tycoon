using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EntityObjectPool : MonoBehaviour
{
    private ObjectPool<GameObject> _pool;

    private GameObject _prefab;

    private Vector3 _spawnPoint;

    public EntityObjectPool(GameObject prefab, int objectsCount)
    {
        this._prefab = prefab;
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroyEntity, false, objectsCount);
    }

    public GameObject Get(Vector3 position)
    {
        _spawnPoint = position;
        GameObject obj = _pool.Get();
        GameEntityRegistry.Instance.EntityRegister(obj.GetComponent<GameEntity>());
        GameEntityRegistry.Instance.EmployeeRegister(obj.GetComponent<Employee>());

        return obj;
    }

    public void Release(GameObject obj)
    {
        _pool.Release(obj);
        GameEntityRegistry.Instance.EntityUnregister(obj.GetComponent<GameEntity>());
        GameEntityRegistry.Instance.EmployeeUnregister(obj.GetComponent<Employee>());
    }

    private GameObject OnCreate()
    {
        GameObject obj = GameObject.Instantiate(_prefab, _spawnPoint, Quaternion.identity);
        return obj;
    }

    private void OnGet(GameObject obj)
    {
        obj.transform.position = _spawnPoint;
        obj.gameObject.SetActive(true);
    }

    private void OnRelease(GameObject obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyEntity(GameObject obj)
    {
        GameObject.Destroy(obj.gameObject);
    }
}
