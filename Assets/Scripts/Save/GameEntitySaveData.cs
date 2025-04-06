using System;

[Serializable]
public class GameEntitySaveData
{
    public int level;
    public int currentUpgradeCost;
    public int upgradeMultiplier;
    public bool isFactory;

    //Sawmill
    public int currentWood;
    public int maxWood;
}
