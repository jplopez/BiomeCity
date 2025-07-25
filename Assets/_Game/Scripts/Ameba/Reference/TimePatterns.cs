using System.Collections;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// TimePatterns.cs — A reference cheat sheet for common time-based execution strategies in Unity.
/// Includes examples of Update-based timers, frame counters, coroutine loops, physics-driven timing,
/// ScriptableObject-based reusable timers, and event-triggered logic.
/// </summary>

#region Regular Timers

/// <summary>
/// Executes an action every X seconds using a Time.deltaTime accumulator inside Update.
/// </summary>
public class TimeAccumulatorTimer : MonoBehaviour {
  /// <summary>Interval (in seconds) between each execution.</summary>
  public float interval = 1f;
  /// <summary>Tracks elapsed time using Time.deltaTime.</summary>
  private float timer = 0f;

  void Update() {
    timer += Time.deltaTime;
    if (timer >= interval) {
      Debug.Log("TimeAccumulatorTimer triggered");
      timer = 0f;
    }
  }
}

/// <summary>
/// Executes an action every Y frames using a frame counter.
/// Useful for lightweight periodic updates tied to rendering cycles.
/// </summary>
public class FrameBasedTimer : MonoBehaviour {
  public int frameInterval = 30;
  private int frameCount = 0;

  void Update() {
    frameCount++;
    if (frameCount >= frameInterval) {
      Debug.Log("FrameBasedTimer triggered");
      frameCount = 0;
    }
  }
}

/// <summary>
/// Executes an action every X seconds using FixedUpdate.
/// Best for physics-driven timing where consistency across frames is important.
/// </summary>
public class FixedUpdateTimer : MonoBehaviour {
  public float physicsInterval = 1f;
  private float physicsTimer = 0f;

  void FixedUpdate() {
    physicsTimer += Time.fixedDeltaTime;
    if (physicsTimer >= physicsInterval) {
      Debug.Log("FixedUpdateTimer triggered");
      physicsTimer = 0f;
    }
  }
}

#endregion

#region Coroutine Timers

/// <summary>
/// Uses a coroutine loop with WaitForSeconds to trigger actions every X seconds.
/// Ideal for background processes and decoupled timing logic.
/// </summary>
public class CoroutineTimer : MonoBehaviour {
  public float interval = 1f;

  void Start() {
    StartCoroutine(RunEveryXSeconds(interval));
  }

  IEnumerator RunEveryXSeconds(float interval) {
    while (true) {
      Debug.Log("CoroutineTimer triggered");
      yield return new WaitForSeconds(interval);
    }
  }
}

/// <summary>
/// Similar to CoroutineTimer, but uses WaitForSecondsRealtime.
/// Ignores Time.timeScale, so it works in pause menus or slow-mo effects.
/// </summary>
public class RealtimeTimer : MonoBehaviour {
  public float interval = 1f;

  void Start() {
    StartCoroutine(RunEveryXRealSeconds(interval));
  }

  IEnumerator RunEveryXRealSeconds(float interval) {
    while (true) {
      Debug.Log("RealtimeTimer triggered");
      yield return new WaitForSecondsRealtime(interval);
    }
  }
}

#endregion

#region Timer Class (Reusable)

/// <summary>
/// Lightweight and reusable Timer class.
/// Can be plugged into any MonoBehaviour with manual ticking.
/// </summary>
public class Timer {
  float elapsed = 0f;
  float duration;

  public Timer(float duration) => this.duration = duration;

  /// <summary>
  /// Adds deltaTime to the elapsed time and resets if duration reached.
  /// Returns true on trigger.
  /// </summary>
  public bool Tick(float deltaTime) {
    elapsed += deltaTime;
    if (elapsed >= duration) {
      elapsed = 0f;
      return true;
    }
    return false;
  }

  /// <summary>Manually reset the timer.</summary>
  public void Reset() => elapsed = 0f;
}

/// <summary>
/// Sample MonoBehaviour that uses the Timer class to execute logic every X seconds.
/// </summary>
public class TimerUser : MonoBehaviour {
  private Timer myTimer;

  void Start() => myTimer = new Timer(2f);

  void Update() {
    if (myTimer.Tick(Time.deltaTime)) {
      Debug.Log("TimerUser triggered");
    }
  }
}

#endregion

#region ScriptableObject Timer

/// <summary>
/// ScriptableObject-based timer for reusable, inspector-editable timing logic.
/// Ideal for shared timers across systems or clean asset-based modularity.
/// </summary>
[CreateAssetMenu(menuName = "Timers/BasicTimer")]
public class TimerSO : ScriptableObject {
  public float interval = 1f;
  private float elapsed = 0f;

  public bool Tick(float deltaTime) {
    elapsed += deltaTime;
    if (elapsed >= interval) {
      elapsed = 0f;
      return true;
    }
    return false;
  }

  public void Reset() => elapsed = 0f;
}

/// <summary>
/// Sample MonoBehaviour that ticks a ScriptableObject timer every frame.
/// Triggers logic when the timer interval is reached.
/// </summary>
public class SOUser : MonoBehaviour {
  public TimerSO timerSO;

  void Update() {
    if (timerSO != null && timerSO.Tick(Time.deltaTime)) {
      Debug.Log("SOUser triggered");
    }
  }
}

#endregion

#region Event-Driven Timer

/// <summary>
/// Timer that starts and stops based on external events.
/// Triggers UnityEvent on interval. Controlled by other scripts or Unity lifecycle.
/// </summary>
public class EventTimer : MonoBehaviour {
  public float interval = 1f;
  public UnityEvent onTick;
  private Coroutine timerRoutine;

  /// <summary>Starts the timer coroutine loop.</summary>
  public void StartTimer() {
    if (timerRoutine == null)
      timerRoutine = StartCoroutine(TimerLoop());
  }

  /// <summary>Stops the timer coroutine loop.</summary>
  public void StopTimer() {
    if (timerRoutine != null) {
      StopCoroutine(timerRoutine);
      timerRoutine = null;
    }
  }

  IEnumerator TimerLoop() {
    while (true) {
      yield return new WaitForSeconds(interval);
      onTick?.Invoke();
    }
  }
}

/// <summary>
/// Sample trigger script that starts the EventTimer on enable and stops it on disable.
/// Great for lifecycle-driven UI, spawning, or toggled behaviors.
/// </summary>
public class EventTriggerExample : MonoBehaviour {
  public EventTimer eventTimer;

  void OnEnable() => eventTimer?.StartTimer();
  void OnDisable() => eventTimer?.StopTimer();
}

#endregion