using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraSettings
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _minZoom;
    [SerializeField] private float _maxZoom;

    public float MoveSpeed => _moveSpeed;
    public float ZoomSpeed => _zoomSpeed;
    public float MinZoom => _minZoom;
    public float MaxZoom => _maxZoom;

}
