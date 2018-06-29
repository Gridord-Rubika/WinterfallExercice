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
    [Range(0,1)]
    [SerializeField] float walkingPercentageNotAccelerating;
    [SerializeField] AnimationCurve staminaUsed;
    [SerializeField] List<SpeedTierValues> speedTierValues;

    private int _currentSpeedTier;
    private float _currentMinSpeed;
    private float _currentMaxSpeed;
    private float _currentSpeed;
    private bool _isAccelerating = false;
    private Vector3 _direction;

    private StaminaSystem _stamina;
    

    public delegate void IncreaseSpeedTierHandler();
    public event IncreaseSpeedTierHandler SpeedTierIncreased;

    public delegate void DecreaseSpeedTierHandler();
    public event DecreaseSpeedTierHandler SpeedTierDecreased;

    public delegate void ForcedStopHandler();
    public event ForcedStopHandler ForcedStop;

    void Start ()
    {
        _stamina = GetComponent<StaminaSystem>();

        if(_stamina != null) {
            _stamina.StaminaStateChanged += StamineStateChangedHandler;
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
        float newSpeed = _currentSpeed;
        if (_isAccelerating) {
            newSpeed += acceleration * Time.deltaTime;
        } else if(_currentSpeedTier == 0 && _direction != Vector3.zero){
            float speedGoal = _currentMaxSpeed * walkingPercentageNotAccelerating;
            if (newSpeed <= speedGoal) {
                newSpeed = Mathf.Min(newSpeed + acceleration * Time.deltaTime, speedGoal);
            } else {
                newSpeed = Mathf.Max(newSpeed - acceleration * Time.deltaTime, speedGoal);
            }
        } else {
            newSpeed -= acceleration * Time.deltaTime;
        }
        newSpeed = Mathf.Clamp(newSpeed, _currentMinSpeed, _currentMaxSpeed);

        if (_stamina != null) {
            if (_stamina.TryUseStamina(staminaUsed.Evaluate(_currentSpeed) * Time.deltaTime)) {
                _currentSpeed = newSpeed;
            } else {
                _currentSpeed = 0;
            }
        } else {
            _currentSpeed = newSpeed;
        }
    }

    public void TryIncreaseSpeedTier(Vector3 direction)
    {
        if (_currentSpeedTier < speedTierValues.Count - 1 && _currentSpeedTier >= 0) {
            if (_currentSpeed == _currentMaxSpeed) {
                if (SpeedTierIncreased != null) {
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
                if (SpeedTierDecreased != null) {
                    SpeedTierDecreased.Invoke();
                }
                _currentSpeedTier--;
                _currentMinSpeed = speedTierValues[_currentSpeedTier].minSpeed;
                _currentMaxSpeed = speedTierValues[_currentSpeedTier].maxSpeed;
            }
        }
    }

    public void TryForcedStop()
    {
        if (_currentSpeedTier > 0) {
            ResetSpeed();
            ForcedStop.Invoke();
        }
    }

    private void StamineStateChangedHandler(StaminaStates newState)
    {
        if (newState == StaminaStates.EXHAUSTED) {
            ResetSpeed();
        }
    }

    private void ResetSpeed()
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

    public void SetIsAccelerating(bool isAccelerating)
    {
        _isAccelerating = isAccelerating;
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }

}
