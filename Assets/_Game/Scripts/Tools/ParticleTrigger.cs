using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System;
using UnityEngine;

namespace Ameba.Tools {
  public class ParticleTrigger : MonoBehaviour {
    public ParticleSystem Message;

    public LayerMask TargetLayer = LayerManager.PlayerLayerMask;

    protected void Trigger(GameObject other, bool trigger = true) {
      if (TargetLayer.MMContains(other.layer) && Message != null) {
        if (trigger) {
          Message.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
          Message.Play();
        }
        else {
          if(Message.isPlaying) Message.Stop();
        }
      }
    }

    private void OnTriggerEnter2D(Collider2D other) => Trigger(other.gameObject);

    private void OnTriggerExit2D(Collider2D other) => Trigger(other.gameObject, false);

  }
}