using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour {

    private PlayerController _controller;
    private SpeedSystem _speed;

    private bool incPressed = false;
    private bool decPressed = false;

    void Start () {
        _controller = GetComponent<PlayerController>();
        _speed = GetComponent<SpeedSystem>();
    }
	
	void Update () {

        if (Input.GetAxis("Forward") > 0) {
            _speed.SetIsGoingForward(true);
        } else {
            _speed.SetIsGoingForward(false);
        }

        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        _controller.SetDirection(direction);

        if (Input.GetAxisRaw("IncreaseSpeedTier") == 1 && !incPressed) {
            _speed.TryIncreaseSpeedTier(direction);
            incPressed = true;
        }
        else if(Input.GetAxisRaw("IncreaseSpeedTier") == 0) {
            incPressed = false;
        }

        if (Input.GetAxisRaw("DecreaseSpeedTier") == 1 && !decPressed) {
            _speed.TryDecreaseSpeedTier();
            decPressed = true;
        }
        else if (Input.GetAxisRaw("DecreaseSpeedTier") == 0) {
            decPressed = false;
        }

    }
}
