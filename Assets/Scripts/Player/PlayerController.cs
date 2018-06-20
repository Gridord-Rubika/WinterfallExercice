using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    
    [System.Serializable]
    private struct SpeedTierValues
    {
        public float minSpeed;
        public float maxSpeed;
    }

    [HideInInspector]
    public bool isGoingForward = false;
    [HideInInspector]
    public Vector3 direction;

    [SerializeField] float acceleration;
    [SerializeField] float directionThreshold;
    [SerializeField] AnimationCurve staminaUsed;
    [SerializeField] int speedTierFromWhereOnlyForward;
    [SerializeField] List<SpeedTierValues> speedTierValues;

    private Rigidbody _rb;
    private PlayerAnimation _animation;
    private Stamina _stamina;

    private int _currentSpeedTier;
    private float _currentMinSpeed;
    private float _currentMaxSpeed;
    private float _currentSpeed;

    private Vector3 oldDirection;

    void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
        }

        _animation = GetComponent<PlayerAnimation>();
        _stamina = GetComponent<Stamina>();

        _stamina.ExhaustedChanged += ExhaustedHandler;

        _currentSpeedTier = 0;
        if (speedTierValues.Count > 0) {
            _currentMinSpeed = speedTierValues[0].minSpeed;
            _currentMaxSpeed = speedTierValues[0].maxSpeed;
        } else {
            _currentMinSpeed = 0;
            _currentMaxSpeed = 0;
        }

        _currentSpeed = 0;

        direction = Vector3.forward;
        oldDirection = direction;
    }
	
	void FixedUpdate ()
    {
        CalculateSpeed();

        Move();
    }

    private void CalculateSpeed()
    {
        if (isGoingForward){
            _currentSpeed += acceleration * Time.deltaTime;
        } else {
            _currentSpeed -= acceleration * Time.deltaTime;
        }

        _currentSpeed = Mathf.Clamp(_currentSpeed, _currentMinSpeed, _currentMaxSpeed);
    }

    private void Move()
    {
        if (_currentSpeed != 0)
        {
            if (_stamina.UseStamina(staminaUsed.Evaluate(_currentSpeed) * Time.deltaTime))
            {
                if (direction.sqrMagnitude < directionThreshold)
                {
                    _rb.MovePosition(transform.position + Quaternion.LookRotation(oldDirection, Vector3.up) * transform.forward * _currentSpeed * Time.deltaTime);
                }
                else
                {
                    if(_currentSpeedTier >= speedTierFromWhereOnlyForward)
                    {
                        direction.z = 1;
                    }
                    _rb.MovePosition(transform.position + Quaternion.LookRotation(direction, Vector3.up) * transform.forward * _currentSpeed * Time.deltaTime);
                    _animation.Rotate(direction);
                    oldDirection = direction;
                }
                _animation.SetMoving(true);
            }
            else
            {
                _animation.SetMoving(false);
            }
        }
        else
        {
            _animation.SetMoving(false);
        }
    }

    public void TryIncreaseSpeedTier()
    {
        if(_currentSpeedTier < speedTierValues.Count - 1 && _currentSpeedTier >= 0) {
            if (_currentSpeed == _currentMaxSpeed && direction.z > 0.9f) {
                _animation.IncreaseSpeedTier();
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
                _animation.DecreaseSpeedTier();
                _currentSpeedTier--;
                _currentMinSpeed = speedTierValues[_currentSpeedTier].minSpeed;
                _currentMaxSpeed = speedTierValues[_currentSpeedTier].maxSpeed;
            }
        }
    }

    public void ExhaustedHandler(bool isExhausted)
    {
        if(speedTierValues.Count > 0)
        {
            _currentSpeedTier = 0;
            _currentSpeed = 0;
            _currentMinSpeed = speedTierValues[0].minSpeed;
            _currentMaxSpeed = speedTierValues[0].maxSpeed;
        }
    }
}
