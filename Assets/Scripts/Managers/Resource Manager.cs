using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private string SavePath => Path.Combine(Application.persistentDataPath, "resources_save.json");

    public PlayerResources Resources = new();

    [SerializeField] private UIManager _uiManager;

    public int Wood
    {
        get => Resources.wood;
        set
        {
            Resources.wood = value;
            OnWoodChanged();
        }
    }

    public int Money
    {
        get => Resources.money;
        set
        {
            Resources.money = value;
            OnMoneyChanged();
        }
    }

    public int Food
    {
        get => Resources.food;
        set
        {
            Resources.food = value;
            OnFoodChanged();
        }
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        LoadResources();
    }

    private void Start()
    {
        StartCoroutine(IncreaseGold());
    }

    public void SaveResources()
    {
        string json = JsonUtility.ToJson(Resources);
        File.WriteAllText(SavePath, json);
    }

    public void LoadResources()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            Resources = JsonUtility.FromJson<PlayerResources>(json);

            if (Resources.money == 0 && Resources.food == 0 && Resources.wood == 0)
            {
                Wood = 5000;
                Money = 500;
                Food = 100;
            }
            else
            {
                Wood = Resources.wood;
                Money = Resources.money;
                Food = Resources.food;
            }
        }
        else
        {
            Wood = 5000;
            Money = 500;
            Food = 100;
        }
    }

    private void OnWoodChanged()
    {
       _uiManager.UpdateWoodUI(Wood);
    }

    private void OnMoneyChanged()
    {
        _uiManager.UpdateMoneyUI(Money);
    }

    private void OnFoodChanged()
    {
        _uiManager.UpdateFoodUI(Food);
    }

    private IEnumerator IncreaseGold()
    {
        WaitForSeconds waitTime = new WaitForSeconds(1f);

        while (true)
        {
            yield return waitTime;
            Money++;
        }
    }

    private void OnApplicationQuit()
    {
        SaveResources();
    }
}
