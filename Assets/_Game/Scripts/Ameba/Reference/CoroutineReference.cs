using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// <para>
/// <b><u> 🔧 CoroutineReference.cs </u></b>
/// </para>
/// <para>
/// This script serves as a centralized reference implementation for learning, testing,
/// and integrating <b>Coroutines into Unity workflows</b>. It includes examples for:
/// <list type="bullet">
/// <item><b>Tracking coroutine lifecycle</b> with references and flags</item>
/// <item><b>Iterating through lists</b> with delegated actions for extensibility</item>
/// <item><b>Triggering coroutines via UnityEditor</b> via ContextMenu and custom Inspector</item>
/// <item><b>Composing coroutine sequences</b>, loops, and delayed actions</item>
/// </list>
/// </para>
/// <para>
/// Ideal for use in dev tooling, prototyping animation flows, or scripting flexible runtime behaviors.
/// </para>
/// </summary>
/// 
/// <author> Juan Pablo Lopez - jp.lopez.navarro@gmail.com </author>
/// <version>1.0 - 7/24/2025 </version>
/// 
public class CoroutineReference : MonoBehaviour {
  public List<GameObject> targets;  // 🗂 List of targets for delegated actions
  public bool debugMode = true;     // 🔍 Toggle for enabling debug triggers

  Coroutine currentRoutine;         // 🧵 Holds reference to current running coroutine
  bool isRunning;                   // 📍 Tracks if the coroutine is active

  /// <summary>
  /// Starts MyRoutine only if it isn't already running.
  /// </summary>
  void StartRoutine() {
    if (currentRoutine == null) {
      currentRoutine = StartCoroutine(MyRoutine());
    }
  }

  /// <summary>
  /// Stops MyRoutine if it's running and clears the reference.
  /// </summary>
  void StopRoutine() {
    if (currentRoutine != null) {
      StopCoroutine(currentRoutine);
      currentRoutine = null;
    }
  }

  /// <summary>
  /// Sample coroutine that runs for 1 second and toggles the running flag.
  /// </summary>
  IEnumerator MyRoutine() {
    isRunning = true;

    Debug.Log("Coroutine started");
    yield return new WaitForSeconds(1f);
    Debug.Log("Coroutine done");

    isRunning = false;
  }

  /// <summary>
  /// Allows triggering MyRoutine from the Inspector via context menu (Play Mode only).
  /// </summary>
  [ContextMenu("Start Debug Coroutine")]
  public void StartDebugCoroutineViaContextMenu() {
    if (Application.isPlaying) {
      StartRoutine();
    }
    else {
      Debug.LogWarning("Must be in Play Mode to run coroutines.");
    }
  }

  /// <summary>
  /// Begins iterating through the target list, applying HandleTarget with delays.
  /// </summary>
  public void StartDelegatedListRoutine(float delay) {
    StartCoroutine(ProcessTargets(delay));
  }

  /// <summary>
  /// Coroutine that loops through targets and calls a delegated method per item.
  /// </summary>
  IEnumerator ProcessTargets(float delay) {
    foreach (var target in targets) {
      HandleTarget(target);             // Delegate logic here
      yield return new WaitForSeconds(delay);
    }
  }

  /// <summary>
  /// Replace this with any modular logic needed for each target.
  /// </summary>
  void HandleTarget(GameObject target) {
    Debug.Log($"Processed: {target.name}");
    // 💡 Extend with custom logic or visual effects
  }

  /// <summary>
  /// Invokes a coroutine that iterates through targets with injected action logic.
  /// </summary>
  public void StartActionBasedRoutine(Action<GameObject> action, float delay) {
    StartCoroutine(ProcessWithAction(targets, action, delay));
  }

  /// <summary>
  /// Generic coroutine to process any list with a passed-in Action delegate.
  /// Enables runtime customization and plugin support.
  /// </summary>
  IEnumerator ProcessWithAction<T>(List<T> items, Action<T> action, float delay) {
    foreach (var item in items) {
      action?.Invoke(item);
      yield return new WaitForSeconds(delay);
    }
  }

  /// <summary>
  /// Starts a coroutine that chains multiple sequences together.
  /// </summary>
  public void StartSequencedRoutine() {
    StartCoroutine(Sequence());
  }

  /// <summary>
  /// Composes multiple coroutine steps, ideal for animation flows or staged effects.
  /// </summary>
  IEnumerator Sequence() {
    yield return StartCoroutine(FadeIn());
    yield return new WaitForSeconds(1f);
    yield return StartCoroutine(FadeOut());
  }

  /// <summary>
  /// Placeholder coroutine for a fade-in effect.
  /// </summary>
  IEnumerator FadeIn() {
    Debug.Log("Fade in...");
    yield return new WaitForSeconds(0.5f);
  }

  /// <summary>
  /// Placeholder coroutine for a fade-out effect.
  /// </summary>
  IEnumerator FadeOut() {
    Debug.Log("Fade out...");
    yield return new WaitForSeconds(0.5f);
  }

  /// <summary>
  /// Loops infinitely with a delay between each tick. Ideal for timers or polling.
  /// </summary>
  IEnumerator LoopWithDelay(float delay) {
    while (true) {
      Debug.Log("Tick");
      yield return new WaitForSeconds(delay);
    }
  }

  /// <summary>
  /// Executes an action after a specified delay.
  /// </summary>
  IEnumerator DelayedAction(float delay) {
    yield return new WaitForSeconds(delay);
    Debug.Log("Delayed action triggered");
  }
}