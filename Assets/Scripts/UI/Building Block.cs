using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingBlock : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _moneyCostText;
    [SerializeField] private TMP_Text _woodCostText;
    [SerializeField] private Button _button;

    private int _moneyCost;
    private int _woodCost;

    public Image Image { get => _image; set => _image = value; }
    public TMP_Text Name { get => _name; set => _name = value; }
    public TMP_Text MoneyCostText { get => _moneyCostText; set => _moneyCostText = value; }
    public TMP_Text WoodCostText { get => _woodCostText; set => _woodCostText = value; }
    public Button Button { get => _button; set => _button = value; }

    public int MoneyCost { get => _moneyCost; set => _moneyCost = value; }
    public int WoodCost { get => _woodCost; set => _woodCost = value; }
}
