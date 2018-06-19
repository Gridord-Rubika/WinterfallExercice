using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour {

    private PlayerController _controller;

    private bool incPressed = false;
    private bool decPressed = false;

    void Start () {
        _controller = GetComponent<PlayerController>();
        if(_controller == null) {
            _controller = gameObject.AddComponent<PlayerController>();
        }
    }
	
	void Update () {

        if (Input.GetAxis("Forward") > 0) {
            _controller.isGoingForward = true;
        } else {
            _controller.isGoingForward = false;
        }

        Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _controller.direction = dir;

        if (Input.GetAxisRaw("IncreaseSpeedTier") == 1 && !incPressed) {
            _controller.TryIncreaseSpeedTier();
            incPressed = true;
        }
        else if(Input.GetAxisRaw("IncreaseSpeedTier") == 0) {
            incPressed = false;
        }

        if (Input.GetAxisRaw("DecreaseSpeedTier") == 1 && !decPressed) {
            _controller.TryDecreaseSpeedTier();
            decPressed = true;
        }
        else if (Input.GetAxisRaw("DecreaseSpeedTier") == 0) {
            decPressed = false;
        }

    }
}
