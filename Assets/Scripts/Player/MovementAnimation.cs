using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimation : MonoBehaviour {
    
    [SerializeField] GameObject model;

    private Animator _modelAnimator;

    void Start ()
    {
        _modelAnimator = model.GetComponent<Animator>();
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
        if (_modelAnimator != null)
        {
            _modelAnimator.SetBool("Moving", isMoving);
        }
    }

    public void SetExhausted(bool isExhausted)
    {
        if (_modelAnimator != null)
        {
            _modelAnimator.SetBool("Exhausted", isExhausted);
        }
    }

    public void IncreaseSpeedTier(SpeedTierValues newSpeedTierValues)
    {
        if (_modelAnimator != null)
        {
            _modelAnimator.SetTrigger("IncreaseSpeedTier");
        }
    }

    public void DecreaseSpeedTier(SpeedTierValues newSpeedTierValues)
    {
        if (_modelAnimator != null) {
            _modelAnimator.SetTrigger("DecreaseSpeedTier");
        }
    }

    public void ForcedStop(SpeedTierValues newSpeedTierValues)
    {
        if (_modelAnimator != null) {
            _modelAnimator.SetTrigger("ForcedStop");
        }
    }


    public void Rotate(Vector3 direction)
    {
        model.transform.LookAt(model.transform.position + Quaternion.LookRotation(direction, Vector3.up) * transform.forward);
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
