using System.Collections.Generic;
using UnityEngine;

public class EmployeePools : MonoBehaviour
{
    public static EmployeePools Instance { get; private set; }

    [SerializeField] private int _initialEmployeePoolSize;

    [SerializeField] private GameObject _woodcutterPrefab;
    [SerializeField] private GameObject _loaderPrefab;
    [SerializeField] private GameObject _dealerPrefab;

    private List<GameObject> _employeePrefabs;

    private EntityObjectPool _woodcutterPool;
    private EntityObjectPool _loaderPool;
    private EntityObjectPool _dealerPool;

    public GameObject WoodcutterPrefab => _woodcutterPrefab;
    public GameObject LoaderPrefab => _loaderPrefab;
    public GameObject DealerPrefab => _dealerPrefab;
    public EntityObjectPool WoodcutterPool => _woodcutterPool;
    public EntityObjectPool LoaderPool => _loaderPool;
    public EntityObjectPool DealerPool => _dealerPool;
    public List<GameObject> EmployeePrefabs => _employeePrefabs;

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

        _employeePrefabs = new List<GameObject>
        {
            _woodcutterPrefab,
            _loaderPrefab,
            _dealerPrefab
        };
    }

    private void Start()
    {
        if (_woodcutterPool == null && _loaderPool == null && _dealerPool == null)
        {
            _woodcutterPool = new EntityObjectPool(_woodcutterPrefab, _initialEmployeePoolSize);
            _loaderPool = new EntityObjectPool(_loaderPrefab, _initialEmployeePoolSize);
            _dealerPool = new EntityObjectPool(_dealerPrefab, _initialEmployeePoolSize);
        }
    }
}
