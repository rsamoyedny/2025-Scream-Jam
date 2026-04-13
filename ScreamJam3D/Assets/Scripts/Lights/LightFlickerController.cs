using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightFlickerController : MonoBehaviour
{
    public static void Flicker()
    {
        FlickerLights?.Invoke(new object(), new EventArgs());
    }
    public static EventHandler FlickerLights;
    private static bool _valuesReset = false;

    private byte _state = 0;
    /*
    0 - Awaiting flicker
    1 - Doing flicker

    5 - Reseting flicker to normal
    */

    private float _passiveHoverTimer = 0;
    private float _startingIntensity;
    private Light _light;

    public float _flickerTimerTotalTime = 0.2f;
    public float _numFlickers = 5;

    public bool passiveOccilation = false;
    public float occilationAmount = 0;
    public float occilationTime = 0f;
    private float occilationFactor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Clear all the values on start up
        if (!_valuesReset)
        {
            //FlickerLights = null;
            _valuesReset = true;
        }

        _light = GetComponent<Light>();
        _startingIntensity = _light.intensity;
        FlickerLights += StartFlicker;
        _state = 0;
        _passiveHoverTimer = 0;
        occilationFactor = occilationAmount * _startingIntensity;
    }

    private void OnDestroy()
    {
        FlickerLights -= StartFlicker;
    }

    private void StartFlicker(object sender, EventArgs e)
    {
        if (_state != 0)
        {
            _state = 5;
            StopCoroutine(DoFlicker());
        }
        _state = 1;
        StartCoroutine(DoFlicker());
        _light.intensity = 0;
    }

    private IEnumerator DoFlicker()
    {
        for (int i = 1; i <= _numFlickers; i++)
        {
            if (i % 2 == 0) _light.intensity = 0;
            else _light.intensity = _startingIntensity * (1 - 1/i);
            yield return new WaitForSecondsRealtime(_flickerTimerTotalTime * UnityEngine.Random.Range(0.5f, 3));
        }

        _state = 0;
        _light.intensity = _startingIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (!passiveOccilation) return;

        _passiveHoverTimer += Time.deltaTime * occilationTime;
        _passiveHoverTimer %= Mathf.PI;

        if (_state == 0)
        {
            float sin = Mathf.Sin(_passiveHoverTimer);
            _light.intensity = _startingIntensity - occilationFactor * sin * sin;
        }
    }
}