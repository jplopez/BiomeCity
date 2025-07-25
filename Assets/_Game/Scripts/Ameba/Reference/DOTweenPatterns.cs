using DG.Tweening;
using UnityEngine;

/// <summary>
/// DOTweenPatterns.cs — A reference module demonstrating the most common DOTween patterns in Unity.
/// Covers value tweening, sequencing, callbacks, loops, unscaled time, ScriptableObject presets, and material/shader control.
/// Plug-and-play friendly for modular systems like ComfyUnity and SSUPlayer.
/// </summary>

#region Basic MonoBehaviour Tween Examples

/// <summary>
/// Demonstrates moving an object to a target position over time.
/// Useful for character movement, object transitions, or cinematic cameras.
/// </summary>
public class BasicTweenExample : MonoBehaviour {
  public Vector3 targetPosition = new Vector3(3, 0, 0);
  public float duration = 2f;

  void Start() {
    transform.DOMove(targetPosition, duration);
  }
}

/// <summary>
/// Shows how to loop a scale tween using Yoyo mode.
/// Common use case: UI bounce effects, pulsating icons, or breathing animations.
/// </summary>
public class LoopingTweenExample : MonoBehaviour {
  void Start() {
    transform.DOScale(Vector3.one * 1.2f, 0.5f)
             .SetLoops(-1, LoopType.Yoyo);
  }
}

/// <summary>
/// Demonstrates a sequenced animation flow.
/// Often used in cutscenes, onboarding UIs, or chained feedback effects.
/// </summary>
public class SequenceTweenExample : MonoBehaviour {
  void Start() {
    var seq = DOTween.Sequence();
    seq.Append(transform.DOMoveX(2f, 1f));
    seq.AppendInterval(0.25f);
    seq.Append(transform.DOScale(Vector3.zero, 0.75f));
    seq.OnComplete(() => Debug.Log("Sequence complete"));
  }
}

/// <summary>
/// Tween with a completion callback for triggering logic after animation.
/// Useful for delayed execution, state changes, or transitions.
/// </summary>
public class TweenWithCallback : MonoBehaviour {
  void Start() {
    transform.DOMoveY(3f, 1f).OnComplete(() => Debug.Log("Tween finished"));
  }
}

/// <summary>
/// Tween that updates using unscaled time.
/// Crucial for pause menus, slow-mo mechanics, or time-independent systems.
/// </summary>
public class UnscaledTweenExample : MonoBehaviour {
  void Start() {
    transform.DOMoveZ(2f, 3f).SetUpdate(true);
  }
}

#endregion

#region Tweening Custom Values

/// <summary>
/// Tweening a custom float value manually. Great for non-GameObject logic like scores, timers, or AI.
/// </summary>
public class CustomValueTween : MonoBehaviour {
  float myValue = 0f;

  void Start() {
    DOTween.To(() => myValue, x => myValue = x, 10f, 2f)
           .OnUpdate(() => Debug.Log($"Value: {myValue:F2}"));
  }
}

#endregion

#region Material Property Tween (Shader Effects)

/// <summary>
/// Demonstrates tweening a shader property like dissolve amount.
/// Ideal for SSUPlayer dissolve effects, glow fades, or material transitions.
/// </summary>
public class ShaderTweenExample : MonoBehaviour {
  public Material mat;

  void Start() {
    DOTween.To(() => mat.GetFloat("_DissolveAmount"),
               x => mat.SetFloat("_DissolveAmount", x),
               1f, 1f);
  }
}

#endregion

#region ScriptableObject Tween Preset

/// <summary>
/// A reusable tween definition stored in a ScriptableObject.
/// Lets you share animation settings across multiple objects.
/// </summary>
[CreateAssetMenu(menuName = "TweenData/ScaleTween")]
public class TweenData : ScriptableObject {
  [Tooltip("Duration of the tween in seconds")]
  public float duration = 1f;
  [Tooltip("Target scale to reach")]
  public Vector3 targetScale = Vector3.one;
  [Tooltip("Easing function to apply")]
  public Ease easeType = Ease.OutQuad;

  /// <summary>
  /// Applies the tween to a GameObject's transform.
  /// Use in MonoBehaviours to modularize animation behavior.
  /// </summary>
  public Tween Apply(GameObject go) {
    return go.transform.DOScale(targetScale, duration).SetEase(easeType);
  }
}

/// <summary>
/// Consumes TweenData and applies its configuration on start.
/// Great for designer-driven animations, ComfyUnity presets, or feedback pipelines.
/// </summary>
public class TweenUser : MonoBehaviour {
  public TweenData tweenData;

  void Start() {
    if (tweenData != null) {
      tweenData.Apply(gameObject).OnComplete(() => Debug.Log("TweenUser complete"));
    }
  }
}

#endregion