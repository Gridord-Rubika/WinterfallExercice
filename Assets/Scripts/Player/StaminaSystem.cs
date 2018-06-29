using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StaminaStates
{
    VIGOROUS,
    TIRED,
    EXHAUSTED
}

public class StaminaSystem : MonoBehaviour {

    [SerializeField] int maxStamina;
    [SerializeField] float regenPerSecond;
    [SerializeField] float timeBeforeRegen;
    [Range(0,1)]
    [SerializeField] float percentageTired;
    [SerializeField] float timeOfRestAfterExhaustion;

    private StaminaStates _currentState = StaminaStates.VIGOROUS;
    private float _currentStamina = 0;
    private float _regenTimer = 0;
    private bool _isExhausted = false;
    
    public delegate void StaminaStateHandler(StaminaStates newState);
    public event StaminaStateHandler StaminaStateChanged;

    void Start () {
        _currentStamina = maxStamina;
    }
	
	void Update () {

        if (_regenTimer > 0) {
            _regenTimer -= Time.deltaTime;
        }

        if (_regenTimer <= 0) {
            _currentStamina = Mathf.Clamp(_currentStamina + regenPerSecond * Time.deltaTime, 0, maxStamina);
        }

        float percentage = GetPercentageRemainingStamina();

        if (!_isExhausted) {
            if(percentage < percentageTired) {
                if(_currentState != StaminaStates.TIRED) {
                    _currentState = StaminaStates.TIRED;
                    if (StaminaStateChanged != null) {
                        StaminaStateChanged(_currentState);
                    }
                }
            } else {
                if (_currentState != StaminaStates.VIGOROUS) {
                    _currentState = StaminaStates.VIGOROUS;
                    if (StaminaStateChanged != null) {
                        StaminaStateChanged(_currentState);
                    }
                }
            }
        }

	}

    public bool TryUseStamina(float amountUsed)
    {
        if(amountUsed == 0) {
            return true;
        }

        if(!_isExhausted)
        {
            UseStamina(amountUsed);
            return true;
        }

        return false;
    }

    private void UseStamina(float amountUsed)
    {
        _regenTimer = timeBeforeRegen;

        if (_currentStamina > amountUsed) {
            _currentStamina -= amountUsed;
        } else {
            _currentStamina = 0;
            _isExhausted = true;

            _currentState = StaminaStates.EXHAUSTED;
            if (StaminaStateChanged != null) {
                StaminaStateChanged(_currentState);
            }

            Invoke("RecoveredFromExhaustion", timeOfRestAfterExhaustion);
        }
    }

    private void RecoveredFromExhaustion()
    {
        _isExhausted = false;
    }

    public bool IsExhausted()
    {
        return _isExhausted;
    }

    public StaminaStates GetStaminaState()
    {
        return _currentState;
    }

    public int GetMaxStamina()
    {
        return maxStamina;
    }

    public float GetStamina()
    {
        return _currentStamina;
    }

    public float GetPercentageRemainingStamina()
    {
        float percentage;
        if (maxStamina <= 0) {
            percentage = 0;
        } else {
            percentage = Mathf.Clamp(_currentStamina / maxStamina, 0, 1);
        }

        return percentage;
    }

    public float GetPercentageTired()
    {
        return percentageTired;
    }
}
