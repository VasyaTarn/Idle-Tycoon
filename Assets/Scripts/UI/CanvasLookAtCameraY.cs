using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookAtCameraY : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        Vector3 direction = _cameraTransform.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion rotation = Quaternion.LookRotation(-direction);
            transform.rotation = rotation;
        }
    }
}
