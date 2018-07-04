using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(StaminaSystem))]
[RequireComponent(typeof(SpeedSystem))]
public class MovementController : MonoBehaviour {
    
    private Rigidbody _rb;
    private MovementAnimation _animation;
    private SpeedSystem _speedSystem;
    [SerializeField] CameraSystem _cameraSystem;

    private Vector3 _direction;
    private Vector3 _oldDirection;

    void Start () {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
        }

        _animation = GetComponent<MovementAnimation>();
        _speedSystem = GetComponent<SpeedSystem>();

        _direction = Vector3.forward;
        _oldDirection = _direction;
    }
	
	void FixedUpdate () {
        CalculateDirection();
        
        _animation.Rotate(_direction);

        Move();
    }

    private void CalculateDirection() {
        if (_direction == Vector3.zero) {
            _direction = _oldDirection;
        } else {
            _oldDirection = _direction;
        }
    }

    private void Move() {
        float currentSpeed = _speedSystem.GetCurrentSpeed();
        if (currentSpeed != 0) {
            _rb.MovePosition(transform.position + Quaternion.LookRotation(_direction, Vector3.up) * transform.forward * currentSpeed * Time.deltaTime);
            _animation.SetMoving(true);
            if(_cameraSystem.GetCurrentCameraStateName() == CameraStateName.IDLE) {
                _cameraSystem.ChangeState(_speedSystem.GetCurrentSpeedTierValues().cameraStateName, false);
            }
        } else {
            _animation.SetMoving(false);
            if(_cameraSystem.GetCurrentCameraStateName() != CameraStateName.IDLE) {
                _cameraSystem.ChangeState(CameraStateName.IDLE, false);
            }
        }
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }
}
