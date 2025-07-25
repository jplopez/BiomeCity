using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ameba.SSU {
  public enum SSUPropertyType { Float, Color, Vector, Texture, Int }

  /// <summary>
  /// Class responsible for computing an SSU property value change. 
  /// Out-of-the-box, the Evaluate() method of this class supports the types in the Enum SSUPropertyType. 
  /// If you need to implement more specific logic, this class also provides an Evaluate() method that receives a Func parameter where you can specify the calculation logic
  /// </summary>
  [Serializable]
  public class SSUAnimator {

    public bool Active = true;

    [Tooltip("Short name for display in the SSUAnimator editor.")]
    public virtual string DisplayName => $"{PropertyName} ({PropType})";

    [SerializeReference]
    public string PropertyName;

    public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
    public float Speed = 1f;

    public SSUPropertyType PropType = SSUPropertyType.Float;

    public Color ColorA = Color.white;
    public Color ColorB = Color.black;

    public Vector4 VectorA = Vector4.zero;
    public Vector4 VectorB = Vector4.one;

    public float FloatA = 0f;
    public float FloatB = 1f;

    public int IntA = 0;
    public int IntB = 1;

    public Texture TextureA;
    public Texture TextureB;

    public float Progress => Mathf.Clamp01(_currentTime)*100.0f;

    public bool IsComplete => GetT(_currentTime) >= 1f;
    public bool IsDisabled { get => !Active; set => Active = !value; }

    protected float _duration = 1f;
    protected float _currentTime = 0.0f;


    /// <summary>
    /// The default method for this class to calculate the next value of the property. 
    /// This method support all the types specified in the Enum SSUPropertyType.
    /// </summary>
    /// <param name="time"></param>
    /// <returns>next value for 'PropertyName' </returns>
    /// <exception cref="Exception">If the evaluation fails, like the SSUPropertyType is set to Float but the Float attributes of this class are not set</exception>
    public object Evaluate(float time) {
      return PropType switch {
        SSUPropertyType.Float => Evaluate(time, (time) => Mathf.Lerp(FloatA, FloatB, time)),
        SSUPropertyType.Color => Evaluate(time, (time) => Color.Lerp(ColorA, ColorB, time)),
        SSUPropertyType.Vector => Evaluate(time, (time) => Vector4.Lerp(VectorA, VectorB, time)),
        SSUPropertyType.Int => Evaluate(time, (time) => Mathf.RoundToInt(Mathf.Lerp(IntA, IntB, time))),
        SSUPropertyType.Texture => Evaluate(time, (time) => (_currentTime < 0.5f ? TextureA : TextureB)),
        _ => throw new Exception($"SSUAnimator: Unsupported property PropType '{PropType}'.")
      };
    }

    /// <summary>
    /// Return the new value for the property 'PropertyName' based on the logic in Func.
    /// Use this method if you need to implement specific logic when calculating the next value of a shader property
    /// </summary>
    /// <param name="time"></param>
    /// <param name="callback"></param>
    /// <returns>next value for 'PropertyName'</returns>
    /// <exception cref="Exception">If the execution of Func fails </exception>
    public object Evaluate(float time, Func<float, object> callback) {
      if (callback == null)
        throw new ArgumentNullException(nameof(callback), "Custom evaluation logic is missing");
      if (!HasValidValues())
        throw new Exception($"[SSUAnimator] Invalid SSUAnimator for property '{PropertyName}' and Type: '{PropType}' ");
      try {
        _currentTime = GetT(time);
        return callback?.Invoke(_currentTime);
      }
      catch (Exception ex) {
        Debug.LogError($"[SSUAnimator] Evaluate failed: {ex.Message}");
        Debug.LogException(ex);
        throw;
      }
    }
    /// <summary>
    /// Validates the SSUAnimator can perform Evaluate()
    /// Checks values A & B matching PropType are different between each other and not null (if applies)
    /// </summary>
    /// <returns>true if valid. False otherwise</returns>
    protected virtual bool HasValidValues() {
        
      //A and B values are as expected depending on PropType
      if( (PropType == SSUPropertyType.Float && !FloatA.Equals(FloatB))
      || (PropType == SSUPropertyType.Int && !IntA.Equals(IntB))
      || (PropType == SSUPropertyType.Texture
            && TextureA != null
            && TextureB != null
            && !TextureA.Equals(TextureB))
      || (PropType == SSUPropertyType.Color && !ColorA.Equals(ColorB))
      || (PropType == SSUPropertyType.Vector
            && VectorA != null
            && VectorB != null
            && !VectorA.Equals(VectorB)) ) { 
        return true;
      } else {
        string invalidMsg = PropType switch {
                    SSUPropertyType.Float => $"Floats: ({FloatA} , {FloatB})",
                    SSUPropertyType.Color => $"Colors: ({ColorA} , {ColorB})",
                    SSUPropertyType.Vector => $"Vector: ({VectorA} , {VectorB})",
                    SSUPropertyType.Int => $"Ints: ({IntA} , {IntB})",
                    SSUPropertyType.Texture => $"Textures: ({TextureA} , {TextureB})",
                    _ => $"SSUAnimator: Unsupported property PropType '{PropType}'."
                  };
        Debug.LogWarning($"SSUAnimator: Animator is not setup correctly for type {PropType}: {invalidMsg}");
        return false;
      }
    }

    /// <summary>
    /// Convenient wrapper to calculate time based on Curve
    /// </summary>
    protected virtual float GetT(float time) => Mathf.Clamp01(Curve.Evaluate(time * Speed));
  }

  [CreateAssetMenu(menuName = "SSU/Animator")]
  public class SSUAnimatorAsset : ScriptableObject {
    public List<SSUAnimator> Animations = new();
  }
}