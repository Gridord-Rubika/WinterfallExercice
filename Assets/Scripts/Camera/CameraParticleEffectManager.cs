using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParticleEffectManager : MonoBehaviour {

    private CameraSystem _cameraSystem;

    private GameObject _currentParticleEffect;

    void Start () {
        _cameraSystem = GetComponent<CameraSystem>();
        if (_cameraSystem == null) {
            Debug.LogWarning("No CameraSystem on object with CameraParticleEffectManager : " + gameObject.name);
        } else {
            _cameraSystem.CameraStateChanged += ChangeState;
        }
    }

    public void ChangeState(CameraState newState, bool instantChange)
    {
        if (_currentParticleEffect != null) {
            ParticleSystem[] ps = _currentParticleEffect.GetComponentsInChildren<ParticleSystem>();

            foreach(ParticleSystem p in ps) {
                p.Stop();
            }
        }

        _currentParticleEffect = newState.particleEffect;

        if(_currentParticleEffect != null) {
            ParticleSystem[] ps = _currentParticleEffect.GetComponentsInChildren<ParticleSystem>();
            
            foreach (ParticleSystem p in ps) {
                p.Play();
            }
        }
    }
}
