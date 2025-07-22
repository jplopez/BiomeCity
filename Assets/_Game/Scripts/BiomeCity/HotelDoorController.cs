using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using UnityEngine;

namespace BiomeCity {

  public class HotelDoorController : MonoBehaviour {

    public float StartFadeValue = 0.1f;
    public float EndFadeValue = -3.8f;
    public float Speed = 1f;

    protected Material _doorMaterial;

    protected bool _inRange = false;
    protected int _fadePropertyID;
    protected float _fadeValue;

    private readonly string PROP_ID = "_DirectionalGlowFadeFade";

    void Start() {
      _doorMaterial = GetComponent<SpriteRenderer>().material;
      _fadePropertyID = Shader.PropertyToID(PROP_ID);
      _fadeValue = StartFadeValue;
    }

    void Update() {
      if(_doorMaterial == null) {
        Debug.LogWarning("[HotelDoorController] Can't find references to Material"); return;
      }

      if (_inRange) {
        //Open Door
        if (!IsFadeValueOnEnd(_fadeValue)) {
          //Move fade to 'next' value over time.
          _fadeValue = GetNextFadeValue(_fadeValue);
          //Update value in _doorMaterial.
          _doorMaterial.SetFloat(_fadePropertyID, _fadeValue);
        }
      } else {
        //Close Door
        if (!IsFadeValueOnStart(_fadeValue)) {
          //Move fade to 'previous' value over time.
          _fadeValue = GetPrevFadeValue(_fadeValue);
          //Update value in _doorMaterial.
          _doorMaterial.SetFloat(_fadePropertyID, _fadeValue);
        }
      }
    }

    private bool IsFadeValueOnEnd(float currentFadeValue) => (StartFadeValue < EndFadeValue) ? (currentFadeValue >= EndFadeValue) : (currentFadeValue <= EndFadeValue);

    private bool IsFadeValueOnStart(float currentFadeValue) => (StartFadeValue < EndFadeValue) ? (currentFadeValue <= StartFadeValue) : (currentFadeValue >= StartFadeValue);

    private float GetNextFadeValue(float currentFadeValue) => (StartFadeValue < EndFadeValue) ? (currentFadeValue + (Speed * Time.deltaTime)) : (currentFadeValue - (Speed * Time.deltaTime));

    private float GetPrevFadeValue(float currentFadeValue) => (StartFadeValue < EndFadeValue) ? (currentFadeValue - (Speed * Time.deltaTime)) : (currentFadeValue + (Speed * Time.deltaTime));

    public void OnTriggerEnter2D(Collider2D collision) => _inRange = true;

    public void OnTriggerExit2D(Collider2D collider) => _inRange = false;
 
  }
}