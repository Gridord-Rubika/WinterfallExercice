using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAimCamera : MonoBehaviour
{
    public GameObject targetLookAt;
    public GameObject targetToRotate;
    public float rotateSpeed = 5;
    [SerializeField] float angleOffset;

    private Vector3 _offset;
    private Rigidbody _rb;

    void Start()
    {
        _rb = targetToRotate.GetComponent<Rigidbody>();
        _offset = targetLookAt.transform.position - transform.position;
    }

    void LateUpdate()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        _rb.MoveRotation(Quaternion.Euler(0, targetToRotate.transform.eulerAngles.y + horizontal, 0));

        float desiredAngle = targetLookAt.transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(angleOffset, desiredAngle, 0);
        transform.position = targetLookAt.transform.position - (rotation * _offset);

        transform.LookAt(targetLookAt.transform);
    }
}
