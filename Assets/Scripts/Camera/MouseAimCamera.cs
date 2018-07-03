using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAimCamera : MonoBehaviour
{
    [SerializeField] LayerMask distanceCheckLayerMask;
    [SerializeField] CameraState state;

    private Rigidbody _rb;
    private Camera _cam;
    private CameraState _oldState;
    private CameraState _newState;
    private float _angleX = 0;
    private float _angleY = 0;
    private float _screenShakeX = 0;
    private float _screenShakeY = 0;
    private float _shakeTimeX = 0;
    private float _shakeTimeY = 0;
    private Vector3 _lookAtPosition;
    private bool _transitioning = false;

    private Coroutine _transitionCoroutine;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        if (_cam == null) {
            Debug.LogWarning("No camera on object with MouseAimCamera : " + gameObject.name);
        }
    }

    void Start()
    {
        _rb = state.target.GetComponent<Rigidbody>();
        if(state.target != null) {
            _lookAtPosition = state.target.TransformPoint(state.lookOffset);
        }
    }

    private void FixedUpdate()
    {
        if (_rb != null && state.rotateTarget) {
            _rb.MoveRotation(Quaternion.Euler(0, _angleY, 0));
        }
    }

    void LateUpdate()
    {
        float horizontal = Input.GetAxis("Mouse X") * state.rotateSpeedX;
        float vertical = Input.GetAxis("Mouse Y") * state.rotateSpeedY;

        _angleX = Mathf.Clamp(_angleX - vertical, state.minXAngle, state.maxXAngle);
        _angleY += horizontal;

        _shakeTimeX += Time.deltaTime * state.screenShakeXSpeed;
        _shakeTimeY += Time.deltaTime * state.screenShakeYSpeed;

        _screenShakeX = Mathf.Sin(_shakeTimeX) * state.screenShakeXStrength;
        _screenShakeY = Mathf.Cos(_shakeTimeY) * state.screenShakeYStrength;
                
        Quaternion rotation = Quaternion.Euler(state.angleOffsetX + _angleX + _screenShakeX, state.angleOffsetY + _angleY + _screenShakeY, 0);

        if (!_transitioning) {
            _lookAtPosition = state.target.TransformPoint(state.lookOffset);
        }

        transform.position = _lookAtPosition + (rotation * new Vector3(0,0, -CalculateDistance(_lookAtPosition)));
        
        transform.LookAt(_lookAtPosition);
    }

    private float CalculateDistance(Vector3 lookAtPosition)
    {
        Ray targetToCameraRay = new Ray(lookAtPosition, transform.position - lookAtPosition);
        RaycastHit hit;

        if (Physics.SphereCast(targetToCameraRay, state.cameraRadiusCheck, out hit, state.distance, distanceCheckLayerMask.value, QueryTriggerInteraction.Ignore)){
            return hit.distance;
        } else {
            return state.distance;
        }
    }
    
    public void ChangeState(CameraState newState, bool instantChange)
    {
        if(_transitionCoroutine != null) {
            StopCoroutine(_transitionCoroutine);
        }

        _oldState = state;
        _newState = newState;
        _rb = _newState.target.GetComponent<Rigidbody>();

        if (instantChange) {
            state = _newState;
            _cam.fieldOfView = state.fieldOfView;
            _transitioning = false;
        } else {
            _transitionCoroutine = StartCoroutine(StateTransitionCoroutine());
        }
    }

    private IEnumerator StateTransitionCoroutine()
    {
        _transitioning = true;
        float timer = Time.deltaTime;

        Vector3 tmp = _lookAtPosition;
        
        while(timer < _newState.transitionTime) {
            float t = timer / _newState.transitionTime;

            state = CameraState.Lerp(ref _oldState, ref _newState, t);
            _lookAtPosition = Vector3.Lerp(tmp, _newState.target.TransformPoint(_newState.lookOffset), t);
            _cam.fieldOfView = state.fieldOfView;

            yield return null;
            timer += Time.deltaTime;
        }

        state = _newState;
        _cam.fieldOfView = state.fieldOfView;
        _transitioning = false;
    }
}
