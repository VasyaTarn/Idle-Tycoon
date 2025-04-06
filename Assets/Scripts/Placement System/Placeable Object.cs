using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField] private Material _originalMaterial;
    [SerializeField] private Material _validMaterial;
    [SerializeField] private Material _invalidMaterial;

    [SerializeField] private bool _isPivotOffset;
    [SerializeField] private float _Offset;

    [Header("Save Data")]
    [SerializeField] private string _prefabId;
    public Guid InstanceId { get; set; }

    public Material OriginalMaterial => _originalMaterial;
    public Material ValidMaterial => _validMaterial;
    public Material InvalidMaterial => _invalidMaterial;
    public bool IsPivotOffset => _isPivotOffset;
    public float Offset => _Offset;
    public string PrefabId => _prefabId;

    private void Awake()
    {
        if (InstanceId == Guid.Empty)
        {
            InstanceId = Guid.NewGuid();
        }
    }

    public void SetInstanceId(Guid id)
    {
        InstanceId = id;
    }
}
