using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class MovementInput : MonoBehaviour {

    [SerializeField] float directionThreshold;
    [SerializeField] float timeToForcedStop;

    private MovementController _controller;
    private SpeedSystem _speed;

    private bool _incPressed = false;
    private bool _decPressed = false;
    private float _forcedStopTimer = 0;
    private bool _forcedStopAlreadyCalled = false;

    void Start () {
        _controller = GetComponent<MovementController>();
        _speed = GetComponent<SpeedSystem>();
    }
	
	void Update () {

        if (Input.GetAxis("Forward") > 0) {
            _speed.SetIsAccelerating(true);
        } else {
            _speed.SetIsAccelerating(false);
        }

        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if(direction.sqrMagnitude < directionThreshold * directionThreshold) {
            _controller.SetDirection(Vector3.zero);
            _speed.SetDirection(Vector3.zero);
        } else {
            _controller.SetDirection(direction);
            _speed.SetDirection(direction);
        }

        if (Input.GetAxisRaw("IncreaseSpeedTier") == 1 && !_incPressed) {
            _speed.TryIncreaseSpeedTier(direction);
            _incPressed = true;
        }
        else if(Input.GetAxisRaw("IncreaseSpeedTier") == 0) {
            _incPressed = false;
        }

        if (Input.GetAxisRaw("DecreaseSpeedTier") == 1) {
            if (!_decPressed) {
                _speed.TryDecreaseSpeedTier();
                _decPressed = true;
            }
            _forcedStopTimer += Time.deltaTime;
            if (!_forcedStopAlreadyCalled && _forcedStopTimer >= timeToForcedStop) {
                _speed.TryForcedStop();
                _forcedStopAlreadyCalled = true;
            }
        }
        else if (Input.GetAxisRaw("DecreaseSpeedTier") == 0) {
            _decPressed = false;
            _forcedStopTimer = 0;
            _forcedStopAlreadyCalled = false;
        }

    }
}
