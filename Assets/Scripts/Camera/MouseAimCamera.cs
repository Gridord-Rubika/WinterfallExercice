using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAimCamera : MonoBehaviour
{
    [SerializeField] CameraState state;

    private Rigidbody _rb;
    private Camera _cam;
    private float _angleX = 0;
    private float _angleY = 0;

    void Start()
    {
        _cam = GetComponent<Camera>();
        if(_cam == null) {
            Debug.LogWarning("No camera on object with MouseAimCamera : " + gameObject.name);
        }

        _rb = state.target.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        float horizontal = Input.GetAxis("Mouse X") * state.rotateSpeedX;
        float vertical = Input.GetAxis("Mouse Y") * state.rotateSpeedY;

        _angleY += horizontal;

        if (_rb != null && state.rotateTarget) {
            _rb.MoveRotation(Quaternion.Euler(0, state.angleOffsetY + _angleY, 0));
        }

        _angleX = Mathf.Clamp(_angleX - vertical, state.minXAngle, state.maxXAngle);
        Quaternion rotation = Quaternion.Euler(state.angleOffsetX + _angleX, state.angleOffsetY + _angleY, 0);
        Vector3 lookAtPosition = state.target.TransformPoint(state.lookOffset);
        transform.position = lookAtPosition + (rotation * new Vector3(0,0, -CalculateDistance(lookAtPosition)));
        
        transform.LookAt(lookAtPosition);
    }

    private float CalculateDistance(Vector3 lookAtPosition)
    {
        Ray targetToCameraRay = new Ray(lookAtPosition, transform.position - lookAtPosition);
        RaycastHit hit;

        if (Physics.SphereCast(targetToCameraRay, state.cameraRadiusCheck, out hit, state.distance)){
            return hit.distance;
        } else {
            return state.distance;
        }
    }

    public void ChangeState(CameraState newState)
    {
        state = newState;
        _rb = state.target.GetComponent<Rigidbody>();
    }
}
