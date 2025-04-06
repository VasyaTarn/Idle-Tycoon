using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemBlock : MonoBehaviour
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "inventory_save.json");

    [Header("Craft")]
    [SerializeField] private int _craftWoodCost;
    [SerializeField] private Button _craftButton;

    [Header("Inventory")]
    [SerializeField] private int _itemID;
    [SerializeField] private int _saleCost;
    [SerializeField] private TMP_Text _countText;

    private ItemBlockSaveData _data;

    public int ItemID => _itemID;
    public int Count
    {
        get => _data.count;
        set
        {
            _data.count = value;
            OnCountChanged(value);
        }
    }
    public int SaleCost => _saleCost;

    private void Awake()
    {
        LoadData();
    }

    private void Start()
    {
        _craftButton.onClick.AddListener(OnCraft);
    }

    private void Update()
    {
        if(ResourceManager.Instance.Wood <  _craftWoodCost)
        {
            _craftButton.interactable = false;
        }
        else
        {
            _craftButton.interactable = true;
        }
    }

    private void OnCraft()
    {
        ResourceManager.Instance.Wood -= _craftWoodCost;
        Inventory.Instance.Items[_itemID].Count++;
    }

    private void OnCountChanged(int count)
    {
        _countText.text = "x" + ResourceFormatter.FormatResourceNumber(count);
    }

    private void SaveData()
    {
        InventoryData inventoryData = LoadAllInventoryData();

        bool foundItem = false;
        for (int i = 0; i < inventoryData.items.Count; i++)
        {
            if (inventoryData.items[i].id == _itemID)
            {
                inventoryData.items[i].count = _data.count;
                foundItem = true;
                break;
            }
        }

        if (!foundItem)
        {
            inventoryData.items.Add(new SerializableItemData { id = _itemID, count = _data.count });
        }

        string json = JsonUtility.ToJson(inventoryData);
        File.WriteAllText(SavePath, json);
    }

    private void LoadData()
    {
        InventoryData inventoryData = LoadAllInventoryData();

        _data = new ItemBlockSaveData();
        foreach (var item in inventoryData.items)
        {
            if (item.id == _itemID)
            {
                _data.count = item.count;
                break;
            }
        }

        OnCountChanged(_data.count);
    }

    private static InventoryData LoadAllInventoryData()
    {
        if (!File.Exists(SavePath))
        {
            return new InventoryData { items = new List<SerializableItemData>() };
        }

        string json = File.ReadAllText(SavePath);
        try
        {
            InventoryData data = JsonUtility.FromJson<InventoryData>(json);
            return data ?? new InventoryData { items = new List<SerializableItemData>() };
        }
        catch
        {
            Debug.LogError("Ошибка при загрузке данных инвентаря. Создание нового файла.");
            return new InventoryData { items = new List<SerializableItemData>() };
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }
}
