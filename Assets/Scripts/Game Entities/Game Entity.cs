using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class GameEntity : MonoBehaviour
{
    public static readonly int MaxLevel = 12;

    [SerializeField] private Outline _outline;
    [SerializeField] protected int _level;

    [Header("UI")]
    [SerializeField] private Sprite _image;
    [SerializeField] private string _name;

    [SerializeField] private TMP_Text _levelText;

    protected bool _isFactory;

    protected int _currentUpgradeCost;
    protected int _upgradeMultiplier = 2;

    protected abstract int startUpgradeCost { get; }

    public Outline Outline => _outline;
    public int Level
    {
        get => _level;
        set
        {
            _level = value;
            OnLevelChanged(value);
        }
    }
    public int CurrentUpgradeCost => _currentUpgradeCost;
    public int UpgradeMultiplier => _upgradeMultiplier;
    public bool IsFactory => _isFactory;
    public Sprite Image => _image;
    public string Name => _name;

    // Save
    protected virtual bool ShouldRegister => true;
    protected bool _dataLoaded = false;

    protected virtual void Start()
    {
        if (ShouldRegister)
        {
            if(!_dataLoaded)
            {
                _currentUpgradeCost = startUpgradeCost;
            }

            GameEntityRegistry.Instance.EntityRegister(this);
        }
    }

    public virtual void Improve()
    {
        int tempCost = _currentUpgradeCost * _upgradeMultiplier * Level;
        _currentUpgradeCost = tempCost;

        ResourceManager.Instance.Money -= tempCost;
    }

    private void OnLevelChanged(int level)
    {
        _levelText.text = "Lv. " + level;
    }

    public virtual GameEntitySaveData SaveData()
    {
        return new GameEntitySaveData()
        {
            level = _level,
            currentUpgradeCost = _currentUpgradeCost,
            upgradeMultiplier = _upgradeMultiplier,
            isFactory = _isFactory
        };
    }

    public virtual void LoadData(GameEntitySaveData data)
    {
        _level = data.level;
        _currentUpgradeCost = data.currentUpgradeCost;
        _upgradeMultiplier = data.upgradeMultiplier;
        _isFactory = data.isFactory;

        OnLevelChanged(_level);

        _dataLoaded = true;
    }

    private void OnDestroy()
    {
        if (ShouldRegister)
        {
            GameEntityRegistry.Instance.EntityUnregister(this);
        }
    }
}
