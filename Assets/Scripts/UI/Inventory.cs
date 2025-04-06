using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private ItemBlock[] _itemBlocks;
    private Dictionary<int, ItemBlock> _items = new();
    private int _maxID = 0;

    public ItemBlock[] ItemBlocks => _itemBlocks;
    public Dictionary<int, ItemBlock> Items => _items;
    public int MaxID => _maxID;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (ItemBlock block in _itemBlocks)
        {
            _items.Add(block.ItemID, block);

            if(block.ItemID > _maxID)
            {
                _maxID = block.ItemID;
            }
        }
    }
}
