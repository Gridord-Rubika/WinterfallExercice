using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaDisplayUI : MonoBehaviour {

    [SerializeField] StaminaSystem stamina;

    [SerializeField] Image staminaGauge;
    [SerializeField] Color colorFullStamina;
    [SerializeField] Color colorEmptyStamina;
    [SerializeField] Text StaminaStateText;
    
    void Start () {
		if(stamina != null) {
            stamina.StaminaStateChanged += DisplayState;
            DisplayState(stamina.GetStaminaState());
        } else {
            Debug.LogWarning("No StaminaSystem given to StaminaDisplayUI on object : " + gameObject.name);
        }
        staminaGauge.fillAmount = 1;
    }
	
	void Update () {
		if(stamina != null) {
            float percentage = stamina.GetPercentageRemainingStamina();

            staminaGauge.fillAmount = percentage;
            staminaGauge.color = Color.Lerp(colorEmptyStamina, colorFullStamina, percentage);
        }
	}

    private void DisplayState(StaminaStates newState)
    {
        if(StaminaStateText == null) {
            return;
        }
        switch (newState) {
            case StaminaStates.VIGOROUS: StaminaStateText.text = "Vigorous"; break;
            case StaminaStates.TIRED: StaminaStateText.text = "Tired"; break;
            case StaminaStates.EXHAUSTED: StaminaStateText.text = "Exhausted"; break;
            default: StaminaStateText.text = "N/A"; break;
        }
    }
}
