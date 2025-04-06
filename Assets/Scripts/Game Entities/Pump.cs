using UnityEngine;

public class Pump : GameEntity
{
    [SerializeField] private Water _water;
    protected override int startUpgradeCost => 5;

    private void Update()
    {
        if (!_water.IsRising && _water.transform.position.y > -10f)
        {
            _water.LowerObject(_level);
        }
    }

    public override void Improve()
    {
        _currentUpgradeCost *= 2;
        Level++;
    }
}
