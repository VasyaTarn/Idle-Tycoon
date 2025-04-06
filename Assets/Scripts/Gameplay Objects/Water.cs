using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private float _interval;
    [SerializeField] private float _riseSpeed;
    [SerializeField] private float _riseDuration;
    [SerializeField] private float _maxY;
    private float _minY = -10f;

    [SerializeField] private TMP_Text _timerText;

    private float _timer;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private bool _isRising = false;

    private WaterSaveData _data = new();

    public bool IsRising => _isRising;
    private string SavePath => Path.Combine(Application.persistentDataPath, "water_save.json");

    private void Awake()
    {
        Load();
    }

    private void Start()
    {
        _timer = _interval;
        _startPosition = transform.position;
    }

    private void Update()
    {
        _data.position = transform.position;

        if (!_isRising)
        {
            _timer -= Time.deltaTime;

            _timerText.text = Mathf.CeilToInt(_timer).ToString();

            if (_timer <= 0f)
            {
                _isRising = true;
                StartCoroutine(RiseWater());
                _timer = _interval;
            }
        }
    }

    private IEnumerator RiseWater()
    {
        float riseStartTime = Time.time;
        Vector3 initialPosition = transform.position;

        float nextY = transform.position.y + _data.totalRiseAmount;
        if (nextY > _maxY)
        {
            nextY = _maxY;
        }

        _targetPosition = new Vector3(transform.position.x, nextY, transform.position.z);

        while (true)
        {
            float t = (Time.time - riseStartTime) / _riseDuration;
            t = Mathf.Clamp01(t);

            transform.position = Vector3.Lerp(initialPosition, _targetPosition, t);

            if (t >= 1f) 
            {
                break;
            }

            yield return null;
        }

        _data.totalRiseAmount += 0.1f;

        _isRising = false;
    }

    public void LowerObject(int level)
    {
        float speed = 0.05f * level;
        float newY = Mathf.MoveTowards(transform.position.y, _minY, speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(_data);
        File.WriteAllText(SavePath, json);
    }

    public void Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            _data = JsonUtility.FromJson<WaterSaveData>(json);

            transform.position = new Vector3(0, _data.position.y, 0);
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
