﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(StaminaSystem))]
[RequireComponent(typeof(SpeedSystem))]
public class PlayerController : MonoBehaviour {
    
    [HideInInspector]

    [SerializeField] float directionThreshold;
    [SerializeField] int speedTierFromWhereOnlyForward;

    private Rigidbody _rb;
    private PlayerAnimation _animation;
    private SpeedSystem _speed;

    private Vector3 _direction;
    private Vector3 _oldDirection;

    void Start () {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
        }

        _animation = GetComponent<PlayerAnimation>();
        _speed = GetComponent<SpeedSystem>();

        _direction = Vector3.forward;
        _oldDirection = _direction;
    }
	
	void FixedUpdate () {
        CalculateDirection();


        _animation.Rotate(_direction);

        Move();
    }

    private void CalculateDirection() {
        if (_direction.sqrMagnitude < directionThreshold * directionThreshold) {
            _direction = _oldDirection;
        } else {
            if (_speed.GetCurrentSpeedTier() >= speedTierFromWhereOnlyForward) {
                _direction.z = 1;
            }
            _oldDirection = _direction;
        }
    }

    private void Move() {
        float currentSpeed = _speed.GetCurrentSpeed();
        if (currentSpeed != 0) {
            _rb.MovePosition(transform.position + Quaternion.LookRotation(_direction, Vector3.up) * transform.forward * currentSpeed * Time.deltaTime);
            _animation.SetMoving(true);
        } else {
            _animation.SetMoving(false);
        }
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }
}
