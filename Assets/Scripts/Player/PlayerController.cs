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

    public bool isGoingForward = false;
    [Range(-1, 1)]
    public float rotationDirection = 0;

    [SerializeField] float rotationSpeed;
    [SerializeField] float acceleration;
    [SerializeField] List<SpeedTierValues> speedTierValues;
    
    private Rigidbody _rb;
    private int currentSpeedTier;
    private float currentMinSpeed;
    private float currentMaxSpeed;
    private float currentSpeed;

    void Start () {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null) {
            _rb = gameObject.AddComponent<Rigidbody>();
        }

        currentSpeedTier = 0;
        if (speedTierValues.Count > 0) {
            currentMinSpeed = speedTierValues[0].minSpeed;
            currentMaxSpeed = speedTierValues[0].maxSpeed;
        } else {
            currentMinSpeed = 0;
            currentMaxSpeed = 0;
        }

        currentSpeed = 0;
    }
	
	void FixedUpdate ()
    {
        Rotate();

        CalculateSpeed();

        Move();
    }

    private void Rotate()
    {
        if(rotationDirection != 0) {
            float newY = transform.eulerAngles.y + (rotationDirection * rotationSpeed * Time.deltaTime);
            _rb.MoveRotation(Quaternion.Euler(0, newY, 0));
        }
    }

    private void CalculateSpeed()
    {
        if (isGoingForward){
            currentSpeed += acceleration = Time.deltaTime;
        } else {
            currentSpeed -= acceleration = Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, currentMinSpeed, currentMaxSpeed);
    }

    private void Move()
    {
        if(currentSpeed != 0) {
            _rb.MovePosition(transform.position + transform.forward * currentSpeed * Time.deltaTime);
        }
    }

    public void TryIncreaseSpeedTier()
    {
        if(currentSpeedTier < speedTierValues.Count - 1 && currentSpeedTier >= 0) {
            if(currentSpeed == currentMaxSpeed) {
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
                currentSpeedTier--;
                currentMinSpeed = speedTierValues[currentSpeedTier].minSpeed;
                currentMaxSpeed = speedTierValues[currentSpeedTier].maxSpeed;
            }
        }
    }
}
