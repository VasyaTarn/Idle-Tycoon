using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance { get; private set; }

    [SerializeField] private PlaceableObject[] _placableObjects;
    [SerializeField] private LayerMask _placementLayerMask;
    private KeyCode _placementKey = KeyCode.Mouse0;
    private KeyCode _cancelKey = KeyCode.Mouse1;

    private PlaceableObject _currentObject;
    private bool _canPlace = false; 
    private List<GameEntity> _placedObjects = new(); 
    private MeshRenderer _tempRenderer;

    [SerializeField] private LayerMask _layerMaskForValidPlace;

    private bool _isStartPlactment;
    private int _placeObjectIndex;

    [Header("Buildings UI")]
    [SerializeField] private GameObject _content;
    [SerializeField] private BuildingBlock _buildingBlock;

    [Header("Start Sawmill")]
    [SerializeField] private AbstractFactory _startSawmill;

    public PlaceableObject[] PlaceableObjects => _placableObjects;
    public List<GameEntity> PlacedObjects => _placedObjects;


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
        for(int i = 0; i < _placableObjects.Length; i++)
        {
            BuildingBlock temp = Instantiate(_buildingBlock, _content.transform);

            temp.Image.sprite = _placableObjects[i].GetComponent<GameEntity>().Image;
            temp.Name.text = _placableObjects[i].GetComponent<GameEntity>().Name;
            temp.MoneyCostText.text = ResourceFormatter.FormatResourceNumber(_placableObjects[i].GetComponent<IBuildable>().BuildMoneyCost);
            temp.WoodCostText.text = ResourceFormatter.FormatResourceNumber(_placableObjects[i].GetComponent<IBuildable>().BuildWoodCost);

            temp.MoneyCost = _placableObjects[i].GetComponent<IBuildable>().BuildMoneyCost;
            temp.WoodCost = _placableObjects[i].GetComponent<IBuildable>().BuildWoodCost;

            int index = i;
            temp.Button.onClick.AddListener(() => StartPlacement(index));
        }
    }

    public void StartPlacement(int objectIndex)
    {
        if (objectIndex < 0 || objectIndex >= _placableObjects.Length)
            return;

        CancelPlacement();

        _isStartPlactment = true;
        _placeObjectIndex = objectIndex;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, _placementLayerMask))
        {
            if (_isStartPlactment)
            {
                _currentObject = Instantiate(_placableObjects[_placeObjectIndex]);
                _isStartPlactment = false;

                CollectRenderers(_currentObject);
            }

            if (_currentObject != null)
            {
                Vector3 targetPosition = hit.point;

                float alignmentStrength = 0.2f;
                Vector3 adjustedNormal = Vector3.Slerp(Vector3.up, hit.normal, alignmentStrength);

                Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, adjustedNormal);
                targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, -90, targetRotation.eulerAngles.z);

                float heightOffset = 0.1f;
                targetPosition += hit.normal * heightOffset;

                if (_currentObject.IsPivotOffset)
                {
                    _currentObject.transform.position = new Vector3(targetPosition.x, targetPosition.y + _currentObject.Offset, targetPosition.z);
                }
                else
                {
                    _currentObject.transform.position = targetPosition;
                }

                _currentObject.transform.rotation = targetRotation;

                _canPlace = CheckPlacementValidity();

                SetMaterial(_canPlace ? _currentObject.ValidMaterial : _currentObject.InvalidMaterial);

                if (Input.GetKeyDown(_placementKey) && _canPlace)
                {
                    PlaceObject();
                }
            }
        }

        if (Input.GetKeyDown(_cancelKey) && _currentObject != null)
        {
            CancelPlacement();
        }
    }

    private bool CheckPlacementValidity()
    {
        Collider collider = _currentObject.GetComponent<Collider>();

        Collider[] overlaps = Physics.OverlapBox(
            collider.bounds.center,
            collider.bounds.extents,
            _currentObject.transform.rotation,
            _layerMaskForValidPlace);

        collider.enabled = false;

        bool valid = true;

        foreach (Collider overlap in overlaps)
        {
            if (!overlap.transform.IsChildOf(_currentObject.transform))
            {
                
                valid = false;
            }
        }

        collider.enabled = true;

        return valid;
    }

    private void CollectRenderers(PlaceableObject obj)
    {
        _tempRenderer = obj.GetComponent<MeshRenderer>();
    }

    private void SetMaterial(Material material)
    {
        _tempRenderer.material = material;
    }

    private void PlaceObject()
    {
        _placedObjects.Add(_currentObject.GetComponent<GameEntity>());

        ResourceManager.Instance.Wood -= _currentObject.GetComponent<IBuildable>().BuildWoodCost;
        ResourceManager.Instance.Money -= _currentObject.GetComponent<IBuildable>().BuildMoneyCost;

        GameEntityRegistry.Instance.SawmillFactories = Instance.PlacedObjects.OfType<SawmillFactory>().ToList();

        RestoreOriginalMaterials();
    }

    private void RestoreOriginalMaterials()
    {
        PlaceableObject placeableObject = _currentObject;

        _tempRenderer.material = _currentObject.OriginalMaterial;

        _currentObject = null;
    }

    public void CancelPlacement()
    {
        if (_currentObject != null)
        {
            _isStartPlactment = false;

            Destroy(_currentObject.gameObject);
            _currentObject = null;
        }
    }
}
