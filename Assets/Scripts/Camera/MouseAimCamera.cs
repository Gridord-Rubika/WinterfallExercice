using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAimCamera : MonoBehaviour
{
    [SerializeField] GameObject targetLookAt;
    [SerializeField] GameObject targetToRotate;
    [SerializeField] float rotateSpeedX = 5;
    [SerializeField] float rotateSpeedY = 1;
    [SerializeField] float minXAngle;
    [SerializeField] float maxXAngle;

    private Vector3 _offset;
    private Rigidbody _rb;
    private float angleOffsetX = 0;

    void Start()
    {
        _rb = targetToRotate.GetComponent<Rigidbody>();
        _offset = targetLookAt.transform.position - transform.position;
    }

    void LateUpdate()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeedX;
        _rb.MoveRotation(Quaternion.Euler(0, targetToRotate.transform.eulerAngles.y + horizontal, 0));

        float desiredAngle = targetLookAt.transform.eulerAngles.y;
        angleOffsetX = Mathf.Clamp(angleOffsetX - Input.GetAxis("Mouse Y") * rotateSpeedY, minXAngle, maxXAngle);
        Quaternion rotation = Quaternion.Euler(angleOffsetX, desiredAngle, 0);
        transform.position = targetLookAt.transform.position - (rotation * _offset);

        transform.LookAt(targetLookAt.transform);
    }
}
