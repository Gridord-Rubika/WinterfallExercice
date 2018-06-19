using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour {

    [SerializeField] int maxStamina;
    [SerializeField] float regenPerSecond;
    [SerializeField] float timeOfRestAfterExhaustion;

    private float _currentStamina;
    private bool _isExhausted;

    [SerializeField] Image staminaGauge;
    [SerializeField] Color colorFullStamina;
    [SerializeField] Color colorEmptyStamina;

    void Start () {
        _currentStamina = maxStamina;
        staminaGauge.fillAmount = 1;
        _isExhausted = false;
    }
	
	void Update () {
        _currentStamina += regenPerSecond * Time.deltaTime;
        _currentStamina = Mathf.Clamp(_currentStamina, 0, maxStamina);
        float percentage = _currentStamina / maxStamina;
        staminaGauge.fillAmount = percentage;
        staminaGauge.color = Color.Lerp(colorEmptyStamina, colorFullStamina, percentage);
	}

    public bool UseStamina(float amountUsed)
    {
        if(!_isExhausted && _currentStamina > amountUsed)
        {
            _currentStamina -= amountUsed;
            return true;
        }
        else if(!_isExhausted)
        {
            _currentStamina = 0;
            _isExhausted = true;
            Invoke("RecoveredFromExhaustion", timeOfRestAfterExhaustion);
        }

        return false;
    }

    private void RecoveredFromExhaustion()
    {
        _isExhausted = false;
    }

    public bool IsExhausted()
    {
        return _isExhausted;
    }
}
