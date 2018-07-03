using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpeedTierValues
{
    public float minSpeed;
    public float maxSpeed;
    public CameraStateName cameraStateName;
}

public class SpeedSystem : MonoBehaviour {

    [SerializeField] float acceleration;
    [Range(0,1)]
    [SerializeField] float walkingPercentageNotAccelerating;
    [SerializeField] AnimationCurve staminaUsed;
    [SerializeField] float timeBeforeStaminaOverUsage;
    [SerializeField] float staminaOverUsageAddedPercentage;
    [SerializeField] List<SpeedTierValues> speedTierValues;

    private int _currentSpeedTier;
    private float _currentMinSpeed;
    private float _currentMaxSpeed;
    private float _currentSpeed;

    private bool _isAccelerating = false;
    private Vector3 _direction;
    private float _staminaOverUsageTimer = 0;

    private StaminaSystem _stamina;

    #region Events
    public delegate void IncreaseSpeedTierHandler(SpeedTierValues newSpeedTierValues);
    public event IncreaseSpeedTierHandler SpeedTierIncreased;

    public delegate void DecreaseSpeedTierHandler(SpeedTierValues newSpeedTierValues);
    public event DecreaseSpeedTierHandler SpeedTierDecreased;

    public delegate void ForcedStopHandler(SpeedTierValues newSpeedTierValues);
    public event ForcedStopHandler ForcedStop;
    #endregion

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
        float newSpeed = CalculateSpeed();

        TryGoToSpeed(newSpeed);
    }

    private float CalculateSpeed()
    {
        float newSpeed = _currentSpeed;
        if (_isAccelerating) {
            newSpeed += acceleration * Time.deltaTime;
        } else if (_currentSpeedTier == 0 && _direction != Vector3.zero) {
            float speedGoal = _currentMaxSpeed * walkingPercentageNotAccelerating;
            if (newSpeed <= speedGoal) {
                newSpeed = Mathf.Min(newSpeed + acceleration * Time.deltaTime, speedGoal);
            } else {
                newSpeed = Mathf.Max(newSpeed - acceleration * Time.deltaTime, speedGoal);
            }
        } else {
            newSpeed -= acceleration * Time.deltaTime;
        }
        return Mathf.Clamp(newSpeed, _currentMinSpeed, _currentMaxSpeed);
    }

    private void TryGoToSpeed(float newSpeed)
    {
        if (_stamina != null) {
            if (_stamina.TryUseStamina(CalculateStaminaUsage())) {
                _currentSpeed = newSpeed;
            } else {
                _currentSpeed = 0;
            }
        } else {
            _currentSpeed = newSpeed;
        }
    }

    private float CalculateStaminaUsage()
    {
        float staminaUsage = 0;

        if(_currentSpeed == _currentMaxSpeed) {
            _staminaOverUsageTimer += Time.deltaTime;
        } else {
            _staminaOverUsageTimer = 0;
        }

        if (_staminaOverUsageTimer >= timeBeforeStaminaOverUsage) {
            staminaUsage = staminaUsed.Evaluate(_currentSpeed) * Time.deltaTime * (1 + staminaOverUsageAddedPercentage);
        } else {
            staminaUsage = staminaUsed.Evaluate(_currentSpeed) * Time.deltaTime;
        }

        return staminaUsage;
    }

    public void TryIncreaseSpeedTier(Vector3 direction)
    {
        if (_currentSpeedTier < speedTierValues.Count - 1 && _currentSpeedTier >= 0) {
            if (_currentSpeed == _currentMaxSpeed) {
                _currentSpeedTier++;
                _currentMinSpeed = speedTierValues[_currentSpeedTier].minSpeed;
                _currentMaxSpeed = speedTierValues[_currentSpeedTier].maxSpeed;

                if (SpeedTierIncreased != null) {
                    SpeedTierIncreased.Invoke(speedTierValues[_currentSpeedTier]);
                }
            }
        }
    }

    public void TryDecreaseSpeedTier()
    {
        if (_currentSpeedTier < speedTierValues.Count && _currentSpeedTier > 0) {
            if (_currentSpeed == _currentMinSpeed) {
                _currentSpeedTier--;
                _currentMinSpeed = speedTierValues[_currentSpeedTier].minSpeed;
                _currentMaxSpeed = speedTierValues[_currentSpeedTier].maxSpeed;

                if (SpeedTierDecreased != null) {
                    SpeedTierDecreased.Invoke(speedTierValues[_currentSpeedTier]);
                }
            }
        }
    }

    public void TryForcedStop()
    {
        if (_currentSpeedTier > 0) {
            ResetSpeed();
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
            ForcedStop.Invoke(speedTierValues[_currentSpeedTier]);
        }
    }

    #region Getters/Setters

    public float GetCurrentSpeed()
    {
        return _currentSpeed;
    }

    public int GetCurrentSpeedTier()
    {
        return _currentSpeedTier;
    }

    public SpeedTierValues GetCurrentSpeedTierValues()
    {
        return speedTierValues[_currentSpeedTier];
    }

    public void SetIsAccelerating(bool isAccelerating)
    {
        _isAccelerating = isAccelerating;
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }

    #endregion
}
