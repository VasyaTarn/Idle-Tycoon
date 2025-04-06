using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private MenuPlayModeManager _menuPlayModeManager;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            _menuPlayModeManager.AcivateGameOverCanvase();
        }
    }
}
