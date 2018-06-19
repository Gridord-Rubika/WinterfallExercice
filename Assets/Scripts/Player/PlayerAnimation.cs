using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    
    [SerializeField] GameObject model_1;

    private Animator _model_1_Animator;

    void Start ()
    {
        _model_1_Animator = model_1.GetComponent<Animator>();
    }

    public void SetMoving(bool isMoving)
    {
        if (_model_1_Animator != null)
        {
            _model_1_Animator.SetBool("Moving", isMoving);
        }
    }

    public void IncreaseSpeedTier()
    {
        if (_model_1_Animator != null)
        {
            _model_1_Animator.SetTrigger("IncreaseSpeedTier");
        }
    }

    public void DecreaseSpeedTier()
    {
        if (_model_1_Animator != null)
        {
            _model_1_Animator.SetTrigger("DecreaseSpeedTier");
        }
    }


    public void Rotate(Vector3 direction)
    {
        model_1.transform.LookAt(model_1.transform.position + Quaternion.LookRotation(direction, Vector3.up) * transform.forward);
    }
}
