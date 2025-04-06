using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastPerformer : MonoBehaviour
{
    [SerializeField] private LayerMask _selectableLayerMask;

    private GameEntity _currentlySelectedObject;

    public Action OnSelectedObject;
    public Action OnDeselectedObject;

    public GameEntity CurrentlySelectedObject => _currentlySelectedObject;

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        CastRayFromCursor();
    }

    private void CastRayFromCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _selectableLayerMask))
        {
            GameEntity hitObject = hit.collider.GetComponent<GameEntity>();

            if (Input.GetMouseButtonDown(0))
            {
                SelectObject(hitObject);
            }
        }

        if (Input.GetMouseButtonDown(1)) 
        {
            DeselectObject();
        }
    }

    private void SelectObject(GameEntity obj)
    {
        DeselectObject();

        _currentlySelectedObject = obj;

        OnSelectedObject?.Invoke();

        _currentlySelectedObject.GetComponent<GameEntity>().Outline.enabled = true;

    }

    private void DeselectObject()
    {
        if (_currentlySelectedObject != null)
        {
            _currentlySelectedObject.GetComponent<GameEntity>().Outline.enabled = false;
            _currentlySelectedObject = null;
            OnDeselectedObject?.Invoke();
        }
    }

    public GameEntity GetSelectedObject()
    {
        return _currentlySelectedObject;
    }


}
