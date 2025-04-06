using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlacedFactorySaveData
{
    public string prefabId;
    public string instanceId;
    public Vector3 position;
    public Quaternion rotation;
    public GameEntitySaveData entityData;
}
