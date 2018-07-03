using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraStateName
{
    IDLE,
    WALKING,
    RUNNING,
    SPRINTING,
    CLIMBING,
    OVER_THE_SHOULDER
}

[System.Serializable]
public struct CameraState
{
    public CameraStateName name;
    [Header("Positioning")]
    public Transform target;
    public Vector3 lookOffset;
    public float distance;
    public float cameraRadiusCheck;
    [Header("Rotation")]
    public bool rotateTarget;
    public float rotateSpeedX;
    public float rotateSpeedY;
    public float angleOffsetX;
    public float angleOffsetY;
    public float minXAngle;
    public float maxXAngle;
    [Header("Others")]
    public float fieldOfView;
    [Range(0, 1)] public float screenShakeStrength;
    public float transitionTime;
}

public class CameraSystem : MonoBehaviour {

    [SerializeField] SpeedSystem speed;
    [SerializeField] List<CameraState> cameraStates;
    [SerializeField] CameraStateName startingStateName;

    private MouseAimCamera _mouseAimCamera;
    private int _currentStateIndex;

    void Start () {
        _mouseAimCamera = GetComponent<MouseAimCamera>();
        if(_mouseAimCamera == null) {
            _mouseAimCamera = gameObject.AddComponent<MouseAimCamera>();
        }

        ChangeState(startingStateName);

        speed.SpeedTierIncreased += SpeedTierChangeHandler;
        speed.SpeedTierDecreased += SpeedTierChangeHandler;
        speed.ForcedStop += SpeedTierChangeHandler;
    }

    public void ChangeState(CameraStateName newStateName)
    {
        for(int i = 0; i < cameraStates.Count; i++) {
            if(cameraStates[i].name == newStateName) {
                _mouseAimCamera.ChangeState(cameraStates[i]);
                _currentStateIndex = i;
                return;
            }
        }

        Debug.LogWarning("Given Camera State Name has no state associated : " + newStateName.ToString());
    }

    private void SpeedTierChangeHandler(SpeedTierValues newSpeedTierValues)
    {
        ChangeState(newSpeedTierValues.cameraStateName);
    }

    public CameraState GetCurrentCameraState()
    {
        return cameraStates[_currentStateIndex];
    }

    public CameraStateName GetCurrentCameraStateName()
    {
        return cameraStates[_currentStateIndex].name;
    }
}
