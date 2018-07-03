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
    [Range(1, 179)] public float fieldOfView;
    [Range(0, 90)] public float screenShakeStrength;
    public float transitionTime;

    public static CameraState Lerp(ref CameraState a, ref CameraState b, float t)
    {
        CameraState s = new CameraState {
            target = b.target,
            rotateTarget = b.rotateTarget,
            transitionTime = b.transitionTime,

            lookOffset = Vector3.Lerp(a.lookOffset, b.lookOffset, t),
            distance = Mathf.Lerp(a.distance, b.distance, t),
            cameraRadiusCheck = Mathf.Lerp(a.cameraRadiusCheck, b.cameraRadiusCheck, t),
            rotateSpeedX = Mathf.Lerp(a.rotateSpeedX, b.rotateSpeedX, t),
            rotateSpeedY = Mathf.Lerp(a.rotateSpeedY, b.rotateSpeedY, t),
            angleOffsetX = Mathf.Lerp(a.angleOffsetX, b.angleOffsetX, t),
            angleOffsetY = Mathf.Lerp(a.angleOffsetY, b.angleOffsetY, t),
            minXAngle = Mathf.Lerp(a.minXAngle, b.minXAngle, t),
            maxXAngle = Mathf.Lerp(a.maxXAngle, b.maxXAngle, t),
            fieldOfView = Mathf.Lerp(a.fieldOfView, b.fieldOfView, t),
            screenShakeStrength = Mathf.Lerp(a.screenShakeStrength, b.screenShakeStrength, t)
        };
        
        return s;
    }
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

        ChangeState(startingStateName, true);

        speed.SpeedTierIncreased += SpeedTierChangeHandler;
        speed.SpeedTierDecreased += SpeedTierChangeHandler;
        speed.ForcedStop += SpeedTierChangeHandler;
    }

    public void ChangeState(CameraStateName newStateName, bool instantChange)
    {
        for(int i = 0; i < cameraStates.Count; i++) {
            if(cameraStates[i].name == newStateName) {
                _mouseAimCamera.ChangeState(cameraStates[i], instantChange);
                _currentStateIndex = i;
                return;
            }
        }

        Debug.LogWarning("Given Camera State Name has no state associated : " + newStateName.ToString());
    }

    private void SpeedTierChangeHandler(SpeedTierValues newSpeedTierValues)
    {
        ChangeState(newSpeedTierValues.cameraStateName, false);
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
