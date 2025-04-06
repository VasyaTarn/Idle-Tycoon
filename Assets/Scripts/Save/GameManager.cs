using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private SaveLoadManager saveLoadManager;

    private void Awake()
    {
        saveLoadManager = GetComponent<SaveLoadManager>();
    }

    private void Start()
    {
        StartCoroutine(Load());
    }

    private void SaveGame()
    {
        saveLoadManager.SavePlacedFactories(PlacementSystem.Instance.PlacedObjects);
        saveLoadManager.SaveEmployees(GameEntityRegistry.Instance.Employees);
        saveLoadManager.SaveGame(GameEntityRegistry.Instance.Entities.ToArray());
    }

    private void LoadGame()
    {
        saveLoadManager.LoadPlacedFactories();
        saveLoadManager.LoadEmployees();
        saveLoadManager.LoadGame(GameEntityRegistry.Instance.Entities.ToArray());
    }

    private IEnumerator Load()
    {
        yield return new WaitForSeconds(1f);
        LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
