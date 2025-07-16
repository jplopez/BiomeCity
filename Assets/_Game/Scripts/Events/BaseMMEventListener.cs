using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

namespace Ameba.CorgiEngine.Events {
  public class BaseMMEventListener<TEvent> : CorgiMonoBehaviour, MMEventListener<TEvent> where TEvent : struct {


    public void Awake() => Initialize();

    public void OnValidate() => Initialize();

    protected virtual void OnEnable() => this.MMEventStartListening<TEvent>();

    protected virtual void OnDisable() => this.MMEventStopListening<TEvent>();

    void MMEventListener<TEvent>.OnMMEvent(TEvent eventType) { if (EvalEventType(eventType)) HandleEvent(eventType); }

    /// <summary>
    /// Override this method if you want the Listener to respond to a specific eventType. Otherwise, the listener will handle all the events is notified. 
    /// </summary>
    protected virtual bool EvalEventType(TEvent evt) => true;

    /// <summary>
    /// Override this method to implement the logic of the event listener
    /// </summary>
    protected virtual void HandleEvent(TEvent evt) { }

    protected virtual void Initialize() { }
  }
}

