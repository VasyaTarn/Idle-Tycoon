using System;
using System.Collections.Generic;

[Serializable]
public class ItemBlockSaveData
{
    public int count;
}

[Serializable]
public class SerializableItemData
{
    public int id;
    public int count;
}

[Serializable]
public class InventoryData
{
    public List<SerializableItemData> items = new List<SerializableItemData>();
}
