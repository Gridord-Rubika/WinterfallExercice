using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    
    [SerializeField] GameObject model_1;
    [SerializeField] GameObject model_1_meshRenderer;

    [SerializeField] GameObject model_2;
    [SerializeField] GameObject model_2_meshRenderer1;
    [SerializeField] GameObject model_2_meshRenderer2;

    private Animator _model_1_Animator;
    private Animator _model_2_Animator;

    [SerializeField] int indexToChangeModel;
    private int currentIndex;

    void Start ()
    {
        _model_1_Animator = model_1.GetComponent<Animator>();
        _model_2_Animator = model_2.GetComponent<Animator>();

        currentIndex = 0;

        model_1_meshRenderer.SetActive(true);
        model_2_meshRenderer1.SetActive(false);
        model_2_meshRenderer2.SetActive(false);
    }

    public void SetMoving(bool isMoving)
    {
        if (_model_1_Animator != null)
        {
            _model_1_Animator.SetBool("Moving", isMoving);
        }
        if (_model_2_Animator != null)
        {
            _model_2_Animator.SetBool("Moving", isMoving);
        }
    }

    public void IncreaseSpeedTier()
    {
        currentIndex++;
        
        if(currentIndex == indexToChangeModel)
        {
            model_1_meshRenderer.SetActive(false);
            model_2_meshRenderer1.SetActive(true);
            model_2_meshRenderer2.SetActive(true);
        }

        if (_model_1_Animator != null)
        {
            _model_1_Animator.SetTrigger("IncreaseSpeedTier");
        }
        if (_model_2_Animator != null)
        {
            _model_2_Animator.SetTrigger("IncreaseSpeedTier");
        }
    }

    public void DecreaseSpeedTier()
    {
        currentIndex--;

        if (currentIndex == indexToChangeModel - 1)
        {
            model_1_meshRenderer.SetActive(true);
            model_2_meshRenderer1.SetActive(false);
            model_2_meshRenderer2.SetActive(false);
        }

        if (_model_1_Animator != null)
        {
            _model_1_Animator.SetTrigger("DecreaseSpeedTier");
        }
        if (_model_2_Animator != null)
        {
            _model_2_Animator.SetTrigger("DecreaseSpeedTier");
        }
    }
}
