using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RaycastPerformer _raycastPerformer;

    [Header("Action Bar")]
    [SerializeField] private GameObject _actionBar;

    [SerializeField] private Image _entityImage;
    [SerializeField] private TMP_Text _entityName;

    [SerializeField] private GameObject _levelUpContainer;
    [SerializeField] private Button _levelUp;
    [SerializeField] private TMP_Text _levelText;

    [SerializeField] private TMP_Text _upgradeCostText;
    [SerializeField] private TMP_Text _createMoneyCostText;
    [SerializeField] private TMP_Text _createFoodCostText;

    [SerializeField] private TMP_Text _maxLevel;

    [SerializeField] private GameObject _createEmployeeContainer;
    [SerializeField] private Button _createEmployeeButton;

    [SerializeField] private GameObject _deleteContainer;
    [SerializeField] private Button _deleteButton;

    [Header("Buildings")]
    [SerializeField] private GameObject _content;
    private List<BuildingBlock> _blocks;

    [Header("Resources")]
    [SerializeField] private TMP_Text _wood;
    [SerializeField] private TMP_Text _money;
    [SerializeField] private TMP_Text _food;

    private AbstractFactory _currentFactory;


    private void Start()
    {
        _levelUp.onClick.AddListener(OnLevelUp);
        _createEmployeeButton.onClick.AddListener(OnCreateEmployee);
        _deleteButton.onClick.AddListener(OnDelete);

        _raycastPerformer.OnSelectedObject += OnSelectHandler;
        _raycastPerformer.OnDeselectedObject += OnDeselectHandler;

        _blocks = _content.GetComponentsInChildren<BuildingBlock>().ToList();
    }

    private void Update()
    {
        if (_raycastPerformer.CurrentlySelectedObject != null)
        {
            if (ResourceManager.Instance.Money < _raycastPerformer.CurrentlySelectedObject.CurrentUpgradeCost * _raycastPerformer.CurrentlySelectedObject.UpgradeMultiplier * _raycastPerformer.CurrentlySelectedObject.Level)
            {
                _levelUp.interactable = false;
            }
            else
            {
                _levelUp.interactable = true;
            }

            if (_raycastPerformer.CurrentlySelectedObject is AbstractFactory factory)
            {
                _currentFactory = factory;

                if (ResourceManager.Instance.Money < _currentFactory.CurrentCreateCost * _currentFactory.CreateMultiplier * _currentFactory.Level || ResourceManager.Instance.Food < _currentFactory.FoodCost)
                {
                    _createEmployeeButton.interactable = false;
                }
                else
                {
                    _createEmployeeButton.interactable = true;
                }
            }

            UpdateUI(_raycastPerformer.CurrentlySelectedObject.Level);
        }

        foreach (var block in _blocks)
        {
            if (ResourceManager.Instance.Money < block.MoneyCost || ResourceManager.Instance.Wood < block.WoodCost)
            {
                block.Button.interactable = false;
            }
            else
            {
                block.Button.interactable = true;
            }
        }
    }

    #region Action Bar

    private void OnSelectHandler()
    {
        _actionBar.gameObject.SetActive(true);

        if (_raycastPerformer.CurrentlySelectedObject is IDeletable entity)
        {
            _deleteContainer.SetActive(true);
        }
        else
        {
            _deleteContainer.SetActive(false);
        }
    }

    private void OnDeselectHandler()
    {
        _actionBar.gameObject.SetActive(false);
    }

    private void OnLevelUp()
    {
        _raycastPerformer.CurrentlySelectedObject.Improve();
    }

    private void OnDelete()
    {
        _actionBar.gameObject.SetActive(false);

        if (_raycastPerformer.CurrentlySelectedObject is IDeletable entity)
        {
            entity.DeleteEntity();
        }
    }

    private void OnCreateEmployee()
    {
        if (_currentFactory != null)
        {
            _currentFactory.CreateEmployee();
        }
    }

    public void UpdateUI(int currentLevel)
    {
        bool canUpgrade = currentLevel != GameEntity.MaxLevel;

        _entityImage.sprite = _raycastPerformer.CurrentlySelectedObject.Image;
        _entityName.text = _raycastPerformer.CurrentlySelectedObject.Name;

        _levelUpContainer.gameObject.SetActive(canUpgrade);
        _maxLevel.gameObject.SetActive(!canUpgrade);

        _levelText.text = "Level: " + currentLevel;
        _upgradeCostText.text = ResourceFormatter.FormatResourceNumber(_raycastPerformer.CurrentlySelectedObject.CurrentUpgradeCost * _raycastPerformer.CurrentlySelectedObject.UpgradeMultiplier * _raycastPerformer.CurrentlySelectedObject.Level);

        if (_currentFactory != null)
        {
            _createMoneyCostText.text = ResourceFormatter.FormatResourceNumber(_currentFactory.CurrentCreateCost * _currentFactory.CreateMultiplier * _currentFactory.Level);
            _createFoodCostText.text = ResourceFormatter.FormatResourceNumber(_currentFactory.FoodCost);
        }


        if (_raycastPerformer.CurrentlySelectedObject.IsFactory)
        {
            _createEmployeeContainer.SetActive(true);
        }
        else
        {
            _createEmployeeContainer.SetActive(false);
        }
    }

    #endregion

    #region Resources

    public void UpdateWoodUI(int wood)
    {
        _wood.text = ResourceFormatter.FormatResourceNumber(wood);
    }

    public void UpdateMoneyUI(int money)
    {
        _money.text = ResourceFormatter.FormatResourceNumber(money);
    }

    public void UpdateFoodUI(int food)
    {
        _food.text = ResourceFormatter.FormatResourceNumber(food);
    }

    #endregion
    private void OnDestroy()
    {
        _raycastPerformer.OnSelectedObject -= OnSelectHandler;
        _raycastPerformer.OnDeselectedObject -= OnDeselectHandler;
    }
}
