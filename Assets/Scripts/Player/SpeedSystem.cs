using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSystem : MonoBehaviour {

    [System.Serializable]
    private struct SpeedTierValues
    {
        public float minSpeed;
        public float maxSpeed;
    }

    [SerializeField] float acceleration;
    [SerializeField] float directionForwardThreshold = 0.9f;
    [SerializeField] List<SpeedTierValues> speedTierValues;

    private int _currentSpeedTier;
    private float _currentMinSpeed;
    private float _currentMaxSpeed;
    private float _currentSpeed;
    private bool _isGoingForward;

    
    public delegate void IncreaseSpeedTierHandler();
    public event IncreaseSpeedTierHandler SpeedTierIncreased;
    
    public delegate void DecreaseSpeedTierHandler();
    public event DecreaseSpeedTierHandler SpeedTierDecreased;

    void Start ()
    {
        StaminaSystem stamina = GetComponent<StaminaSystem>();

        if(stamina != null) {
            stamina.ExhaustedChanged += ExhaustedHandler;
        }

        _currentSpeedTier = 0;
        if (speedTierValues.Count > 0){
            _currentMinSpeed = speedTierValues[0].minSpeed;
            _currentMaxSpeed = speedTierValues[0].maxSpeed;
        } else {
            _currentMinSpeed = 0;
            _currentMaxSpeed = 0;
        }

        _currentSpeed = 0;
    }

    private void Update()
    {
        CalculateSpeed();
    }

    public void CalculateSpeed()
    {
        if (_isGoingForward) {
            _currentSpeed += acceleration * Time.deltaTime;
        } else {
            _currentSpeed -= acceleration * Time.deltaTime;
        }

        _currentSpeed = Mathf.Clamp(_currentSpeed, _currentMinSpeed, _currentMaxSpeed);
    }

    public void TryIncreaseSpeedTier(Vector3 direction)
    {
        if (_currentSpeedTier < speedTierValues.Count - 1 && _currentSpeedTier >= 0) {
            if (_currentSpeed == _currentMaxSpeed && direction.z > directionForwardThreshold) {
                if(SpeedTierIncreased != null) {
                    SpeedTierIncreased.Invoke();
                }
                _currentSpeedTier++;
                _currentMinSpeed = speedTierValues[_currentSpeedTier].minSpeed;
                _currentMaxSpeed = speedTierValues[_currentSpeedTier].maxSpeed;
            }
        }
    }

    public void TryDecreaseSpeedTier()
    {
        if (_currentSpeedTier < speedTierValues.Count && _currentSpeedTier > 0) {
            if (_currentSpeed == _currentMinSpeed) {
                if(SpeedTierDecreased != null) {
                    SpeedTierDecreased.Invoke();
                }
                _currentSpeedTier--;
                _currentMinSpeed = speedTierValues[_currentSpeedTier].minSpeed;
                _currentMaxSpeed = speedTierValues[_currentSpeedTier].maxSpeed;
            }
        }
    }

    public void ExhaustedHandler(bool isExhausted)
    {
        if (speedTierValues.Count > 0) {
            _currentSpeedTier = 0;
            _currentSpeed = 0;
            _currentMinSpeed = speedTierValues[0].minSpeed;
            _currentMaxSpeed = speedTierValues[0].maxSpeed;
        }
    }

    public float GetCurrentSpeed()
    {
        return _currentSpeed;
    }

    public int GetCurrentSpeedTier()
    {
        return _currentSpeedTier;
    }

    public void SetIsGoingForward(bool isGoingForward)
    {
        _isGoingForward = isGoingForward;
    }

}
