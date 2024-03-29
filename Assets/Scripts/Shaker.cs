using UnityEngine;
using System.Collections;
using System;

public class Shaker : MonoBehaviour
{
    [Header("Info")]
    private Vector3 _startPos;
    private float _timer;
    private Vector3 _randomPos;

    public bool ShakeOnEnable = false;

    [Header("Settings")]
    [Range(0f, 5f)]
    public float _time = 0.2f;
    [Range(0f, 5f)]
    public float _distance = 0.1f;
    [Range(0f, 0.1f)]
    public float _delayBetweenShakes = 0f;

    private RectTransform rect;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        _startPos = rect.localPosition;
    }

    private void OnValidate()
    {
        if (_delayBetweenShakes > _time)
            _delayBetweenShakes = _time;
    }

    private void OnEnable()
    {
        if (ShakeOnEnable)
        {
            Shake();
        }
    }

    public void Shake()
    {
        StartCoroutine(IShake());
    }
    private IEnumerator IShake()
    {
        _timer = 0f;

        while (_timer < _time)
        {
            _timer += Time.deltaTime;

            _randomPos = _startPos + (UnityEngine.Random.insideUnitSphere * _distance);

            rect.localPosition = _randomPos;

            if (_delayBetweenShakes > 0f)
            {
                yield return new WaitForSeconds(_delayBetweenShakes);
            }
            else
            {
                yield return null;
            }
        }
        rect.localPosition = _startPos;
    }
}