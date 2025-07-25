using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;


namespace Ameba.SSU {

  public enum SSUPlayerStartMode { Disabled, OnEnable, OnStart }

  //Asc=FIFO, Desc=FILO
  public enum SSUPlayerPlayOrder { Ascending, Descending }

  public enum SSUPlayerPlayMode { OneTime, BackAndForth, Looped }

  public class SSUPlayer : MonoBehaviour {

    public bool Debugging = true;

    [Tooltip("whether the Player should start disabled, or automatically start on OnEnable or On Start ")]
    public SSUPlayerStartMode StartMode = SSUPlayerStartMode.OnEnable;
    [Tooltip("Order of execution of the Animators")]
    public SSUPlayerPlayOrder PlayOrder = SSUPlayerPlayOrder.Ascending;

    [Tooltip("time in seconds the player should wait before start playing")]
    public float InitialDelay = 0.0f;

    [Tooltip("Play the animations One time, back and forth, or in a loop")]
    public SSUPlayerPlayMode PlayMode = SSUPlayerPlayMode.OneTime;

    [Tooltip("whether the player should loop forever. Only works for BackAndForth or Looped")]
    public bool PlayForever = false;

    [Tooltip("how many loops should the player do. Ignored if PlayForever is true")]
    public int NumberOfPlays = 0;

    [Tooltip("time in seconds the player should wait in between loops")]
    public float DelayBetweenPlays = 0.0f;

    [SerializeReference]
    public List<SSUAnimator> Animators = new();

    public bool IsPlaying = false;

    [ReadOnly] public SpriteRenderer Renderer;
    [ReadOnly] public MaterialPropertyBlock Block;

    protected Coroutine _coroutine;
    protected int _playsLeft = 0;
    protected float _currentPlayTime = 0.0f;
    protected bool _reverseDirection = false;

    private void OnValidate() => Init();
    private void Awake() => Init();

    public void Init() {
      Debugging = true;
      if (Renderer == null) Renderer = GetComponent<SpriteRenderer>();
      Block ??= new MaterialPropertyBlock();
      if (!PlayForever) {
        _playsLeft = PlayMode == SSUPlayerPlayMode.OneTime ? 1 : NumberOfPlays;
      }
    }

    private void OnEnable() { if (StartMode == SSUPlayerStartMode.OnEnable) Play(); }

    private void Start() { if (StartMode == SSUPlayerStartMode.OnStart) Play(); }

    public void Play() {
      if (IsPlaying) return;
      if (CanPlay()) {
        IsPlaying = true;
        _coroutine = StartCoroutine(PlayAnimations());
      }
    }

    public void Stop() {
      if (IsPlaying) {
        if (_coroutine != null) StopCoroutine(_coroutine);
        IsPlaying = false;
        Block.Clear();
        Renderer.SetPropertyBlock(Block);
        _currentPlayTime = 0.0f;
      }
    }

    void Update() {
      if (Renderer == null || Block == null || Animators == null || !IsPlaying || !CanPlay()) return;
      _currentPlayTime += Time.deltaTime;
    }

    IEnumerator PlayAnimations() {
      yield return new WaitForSeconds(InitialDelay);

      while (CanPlay()) {
        Block.Clear();
        //skip inactive animators early
        var ordered = GetOrderedAnimators().Where(anim => anim.Active).ToList();

        foreach (var anim in ordered) {
          ExecuteAnimator(anim, _currentPlayTime);
          if (anim.IsComplete) anim.Active = false;
        }

        Renderer.SetPropertyBlock(Block);

        //check if we completed a loop
        if (AllAnimatorsInactive()) {
          if (!PlayForever && _playsLeft > 0) {
            _playsLeft--;
            if (PlayMode == SSUPlayerPlayMode.BackAndForth) _reverseDirection = !_reverseDirection;
          }
          ResetAnimators();
          yield return new WaitForSeconds(DelayBetweenPlays);
        }
        yield return null; // continue to next frame
      }

      IsPlaying = false;
    }

    //Runs the Evaluate() method of a single SSUAnimator
    protected virtual void ExecuteAnimator(SSUAnimator animator, float time) {
      //this validations is relevant to inform because it means the animator in not setup
      if (string.IsNullOrEmpty(animator.PropertyName)) {
        Debug.LogWarning("[SSUPlayer] Skipped animation due to missing property name.");
        return;
      }
      //this validations are part of normal logic, do not need logging
      if (animator != null && animator.Active && !animator.IsComplete) {

        try {
          object value = animator.Evaluate(time);
          if (value == null) {
            Debug.LogWarning($"[SSUPlayer] Animation '{animator.PropertyName}' returned null.");
            return;
          }

          switch (value) {
            case float f: Block.SetFloat(animator.PropertyName, f); break;
            case Color c: Block.SetColor(animator.PropertyName, c); break;
            case Vector4 v: Block.SetVector(animator.PropertyName, v); break;
            case int i: Block.SetInt(animator.PropertyName, i); break;
            case Texture t: Block.SetTexture(animator.PropertyName, t); break;
            default:
              Debug.LogWarning($"[SSUPlayer] Unsupported value Type: {value.GetType()}");
              break;
          }

          if (Debugging) {
            Debug.Log($"[SSUPlayer] Animation '{animator.PropertyName}':{animator.PropType} = {animator.Progress}");
          }
        }
        catch (Exception ex) {
          Debug.LogError($"[SSUPlayer] Failed to evaluate animation '{animator.PropertyName}': {ex.Message}");
        }
      }
    }

    private bool CanPlay() {
      bool canPlay = false;
      switch (PlayMode) {
        case SSUPlayerPlayMode.OneTime: canPlay = _playsLeft > 0; break;
        case SSUPlayerPlayMode.BackAndForth: canPlay = (PlayForever) || (_playsLeft > 0); break;
        case SSUPlayerPlayMode.Looped: canPlay = (PlayForever) || (_playsLeft < 0); break;
        default: break;
      }
      return canPlay;
    }

    private List<SSUAnimator> GetOrderedAnimators() =>
            _reverseDirection
              ? Animators.AsEnumerable().Reverse().ToList()
              : Animators;

    private bool AllAnimatorsInactive() => Animators.All(anim => !anim.Active);

    private void ResetAnimators() => Animators.ForEach(anim => anim.Active = true);
  }
}