using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private string saveFilePath;
    private string placedFactoriesFilePath;
    private string employeesFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "game_save.json");
        placedFactoriesFilePath = Path.Combine(Application.persistentDataPath, "placed_factories.json");
        employeesFilePath = Path.Combine(Application.persistentDataPath, "employees.json");
    }

    public void SaveGame(GameEntity[] gameEntities)
    {
        List<GameEntitySaveData> saveDataList = new List<GameEntitySaveData>();

        foreach (var entity in gameEntities)
        {
            saveDataList.Add(entity.SaveData());
        }

        string json = JsonUtility.ToJson(new SaveGameDataWrapper(saveDataList));
        File.WriteAllText(saveFilePath, json);
    }

    public void LoadGame(GameEntity[] gameEntities)
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveGameDataWrapper wrapper = JsonUtility.FromJson<SaveGameDataWrapper>(json);

            for (int i = 0; i < gameEntities.Length && i < wrapper.savedEntities.Count; i++)
            {
                gameEntities[i].LoadData(wrapper.savedEntities[i]);
            }
        }
    }

    [System.Serializable]
    public class SaveGameDataWrapper
    {
        public List<GameEntitySaveData> savedEntities;

        public SaveGameDataWrapper(List<GameEntitySaveData> savedEntities)
        {
            this.savedEntities = savedEntities;
        }
    }

    public void SavePlacedFactories(List<GameEntity> placedFactories)
    {
        var dataList = new List<PlacedFactorySaveData>();

        foreach (var building in placedFactories)
        {
            var placeable = building.GetComponent<PlaceableObject>();
            if (placeable == null) continue;

            var data = new PlacedFactorySaveData
            {
                prefabId = placeable.PrefabId,
                instanceId = placeable.InstanceId.ToString(),
                position = building.transform.position,
                rotation = building.transform.rotation,
                entityData = building.SaveData()
            };

            dataList.Add(data);
        }

        var wrapper = new FullSaveData
        {
            placedFactories = dataList
        };

        var json = JsonUtility.ToJson(wrapper);
        File.WriteAllText(placedFactoriesFilePath, json);
    }

    public void LoadPlacedFactories()
    {
        if (!File.Exists(placedFactoriesFilePath)) return;

        var json = File.ReadAllText(placedFactoriesFilePath);
        var wrapper = JsonUtility.FromJson<FullSaveData>(json);

        foreach (var data in wrapper.placedFactories)
        {
            var prefab = FindPrefabById(data.prefabId);
            if (prefab == null) continue;

            var spawned = Instantiate(prefab, data.position, data.rotation);
            spawned.GetComponent<PlaceableObject>().SetInstanceId(Guid.Parse(data.instanceId));

            var building = spawned;

            building.LoadData(data.entityData);

            PlacementSystem.Instance.PlacedObjects.Add(building);
        }
    }

    private GameEntity FindPrefabById(string id)
    {
        foreach (var obj in PlacementSystem.Instance.PlaceableObjects)
        {
            if (obj.PrefabId == id)
                return obj.GetComponent<GameEntity>();
        }

        return null;
    }

    public void SaveEmployees(List<Employee> employees)
    {
        var employeeDataList = new List<EmployeeSaveData>();

        foreach (var employee in employees)
        {
            var placeableFactory = employee.Factory?.GetComponent<PlaceableObject>();

            var data = new EmployeeSaveData
            {
                employeeId = employee.PrefabId,
                position = employee.transform.position,
                factoryId = placeableFactory != null ? placeableFactory.InstanceId.ToString() : null
            };

            employeeDataList.Add(data);
        }

        var json = JsonUtility.ToJson(new EmployeeSaveDataWrapper { employees = employeeDataList });
        File.WriteAllText(employeesFilePath, json);
    }

    public void LoadEmployees()
    {
        GameEntityRegistry.Instance.SawmillFactories = PlacementSystem.Instance.PlacedObjects.OfType<SawmillFactory>().ToList();
        if (!File.Exists(employeesFilePath)) return;

        var json = File.ReadAllText(employeesFilePath);
        var wrapper = JsonUtility.FromJson<EmployeeSaveDataWrapper>(json);

        foreach (var data in wrapper.employees)
        {
            var prefab = FindEmployeePrefabById(data.employeeId);
            if (prefab == null)
            {
                continue;
            }

            var factory = PlacementSystem.Instance.PlacedObjects
                .FirstOrDefault(p => p.GetComponent<PlaceableObject>().InstanceId.ToString() == data.factoryId);

            if (prefab is Woodcutter)
            {
                Woodcutter woodcutter = EmployeePools.Instance.WoodcutterPool.Get(factory.transform.position).GetComponent<Woodcutter>();

                if (factory != null)
                {
                    woodcutter.SetFactory(factory.GetComponent<AbstractFactory>());
                    woodcutter.SetStartPosition(factory.transform.position);
                }

                factory.GetComponent<AbstractFactory>().Employees.Add(woodcutter);
                GameEntityRegistry.Instance.Entities.Add(woodcutter);
            }
            else if (prefab is Loader)
            {
                Loader loader = EmployeePools.Instance.LoaderPool.Get(factory.transform.position).GetComponent<Loader>();

                if (factory != null)
                {
                    loader.SetFactory(factory.GetComponent<AbstractFactory>());
                    loader.SetStartPosition(factory.transform.position);
                }

                factory.GetComponent<AbstractFactory>().Employees.Add(loader);
                GameEntityRegistry.Instance.Entities.Add(loader);
            }
            else
            {
                Dealer dealer = EmployeePools.Instance.DealerPool.Get(factory.transform.position).GetComponent<Dealer>();

                if (factory != null)
                {
                    dealer.SetFactory(factory.GetComponent<AbstractFactory>());
                    dealer.SetStartPosition(factory.transform.position);
                }

                factory.GetComponent<AbstractFactory>().Employees.Add(dealer);
                GameEntityRegistry.Instance.Entities.Add(dealer);
            }
        }
    }

    private Employee FindEmployeePrefabById(string id)
    {
        foreach (var obj in EmployeePools.Instance.EmployeePrefabs)
        {
            if (obj.GetComponent<Employee>().PrefabId == id)
                return obj.GetComponent<Employee>();
        }

        return null;
    }
}

[System.Serializable]
public class FullSaveData
{
    public List<PlacedFactorySaveData> placedFactories;
}

[Serializable]
public class EmployeeSaveDataWrapper
{
    public List<EmployeeSaveData> employees;
}
