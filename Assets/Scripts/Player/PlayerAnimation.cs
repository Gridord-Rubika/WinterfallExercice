using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    
    [SerializeField] GameObject model_1;

    private Animator _model_1_Animator;

    void Start ()
    {
        _model_1_Animator = model_1.GetComponent<Animator>();
        StaminaSystem stamina = GetComponent<StaminaSystem>();
        if (stamina != null) {
            stamina.StaminaStateChanged += StamineStateChangedHandler;
        }

        SpeedSystem speed = GetComponent<SpeedSystem>();
        if (speed != null) {
            speed.SpeedTierIncreased += IncreaseSpeedTier;
            speed.SpeedTierDecreased += DecreaseSpeedTier;
            speed.ForcedStop += ForcedStop;
        }
    }

    public void SetMoving(bool isMoving)
    {
        if (_model_1_Animator != null)
        {
            _model_1_Animator.SetBool("Moving", isMoving);
        }
    }

    public void SetExhausted(bool isExhausted)
    {
        if (_model_1_Animator != null)
        {
            _model_1_Animator.SetBool("Exhausted", isExhausted);
        }
    }

    public void IncreaseSpeedTier(SpeedTierValues newSpeedTierValues)
    {
        if (_model_1_Animator != null)
        {
            _model_1_Animator.SetTrigger("IncreaseSpeedTier");
        }
    }

    public void DecreaseSpeedTier(SpeedTierValues newSpeedTierValues)
    {
        if (_model_1_Animator != null) {
            _model_1_Animator.SetTrigger("DecreaseSpeedTier");
        }
    }

    public void ForcedStop(SpeedTierValues newSpeedTierValues)
    {
        if (_model_1_Animator != null) {
            _model_1_Animator.SetTrigger("ForcedStop");
        }
    }


    public void Rotate(Vector3 direction)
    {
        model_1.transform.LookAt(model_1.transform.position + Quaternion.LookRotation(direction, Vector3.up) * transform.forward);
    }

    private void StamineStateChangedHandler(StaminaStates newState)
    {
        if(newState == StaminaStates.EXHAUSTED) {
            SetExhausted(true);
        } else {
            SetExhausted(false);
        }
    }
}
