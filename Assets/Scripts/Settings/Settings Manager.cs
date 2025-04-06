using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private CameraSettings _cameraSettings;

    public CameraSettings CameraSettings => _cameraSettings;
}
