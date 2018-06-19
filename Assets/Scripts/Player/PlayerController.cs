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
    [SerializeField] List<SpeedTierValues> speedTierValues;

    private Rigidbody _rb;
    private PlayerAnimation _animation;

    private int currentSpeedTier;
    private float currentMinSpeed;
    private float currentMaxSpeed;
    private float currentSpeed;

    private Vector3 oldDirection;

    void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
        }

        _animation = GetComponent<PlayerAnimation>();

        currentSpeedTier = 0;
        if (speedTierValues.Count > 0) {
            currentMinSpeed = speedTierValues[0].minSpeed;
            currentMaxSpeed = speedTierValues[0].maxSpeed;
        } else {
            currentMinSpeed = 0;
            currentMaxSpeed = 0;
        }

        currentSpeed = 0;

        direction = transform.forward;
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
            currentSpeed += acceleration * Time.deltaTime;
        } else {
            currentSpeed -= acceleration * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, currentMinSpeed, currentMaxSpeed);
    }

    private void Move()
    {
        if (currentSpeed != 0)
        {
            if (direction.sqrMagnitude < directionThreshold)
            {
                _rb.MovePosition(transform.position + Quaternion.LookRotation(oldDirection, Vector3.up) * transform.forward * currentSpeed * Time.deltaTime);
            }
            else
            {
                _rb.MovePosition(transform.position + Quaternion.LookRotation(direction, Vector3.up) * transform.forward * currentSpeed * Time.deltaTime);
                oldDirection = direction;
            }
            _animation.SetMoving(true);
            _animation.Rotate(oldDirection);
        }
        else
        {
            _animation.SetMoving(false);
        }
    }

    public void TryIncreaseSpeedTier()
    {
        if(currentSpeedTier < speedTierValues.Count - 1 && currentSpeedTier >= 0) {
            if(currentSpeed == currentMaxSpeed) {
                _animation.IncreaseSpeedTier();
                currentSpeedTier++;
                currentMinSpeed = speedTierValues[currentSpeedTier].minSpeed;
                currentMaxSpeed = speedTierValues[currentSpeedTier].maxSpeed;
            }
        }
    }

    public void TryDecreaseSpeedTier()
    {
        if (currentSpeedTier < speedTierValues.Count && currentSpeedTier > 0) {
            if (currentSpeed == currentMinSpeed) {
                _animation.DecreaseSpeedTier();
                currentSpeedTier--;
                currentMinSpeed = speedTierValues[currentSpeedTier].minSpeed;
                currentMaxSpeed = speedTierValues[currentSpeedTier].maxSpeed;
            }
        }
    }
}
