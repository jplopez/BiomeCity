using System;
using System.Collections.Generic;
using UnityEngine;

namespace BiomeCity {
  [ExecuteAlways]
  [RequireComponent(typeof(SpriteRenderer))]
  [AddComponentMenu("Ameba/Building")]
  public class Building : MonoBehaviour {
    [Header("Building Sprite")]
    public Sprite MainSprite;

    [Header("Accessories")]
    public List<IAccessory> Accessories = new();

    private SpriteRenderer _spriteRenderer;

    private void Awake() {
      if (!TryGetComponent(out _spriteRenderer))
        _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
      if (MainSprite != null)
        _spriteRenderer.sprite = MainSprite;
    }

    private void OnValidate() {
      if (_spriteRenderer == null)
        _spriteRenderer = GetComponent<SpriteRenderer>();
      if (_spriteRenderer != null && MainSprite != null)
        _spriteRenderer.sprite = MainSprite;
    }

    private void Reset() {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      if (_spriteRenderer != null)
        MainSprite = _spriteRenderer.sprite;
    }

    private void Start() {
      foreach (var accessory in Accessories)
        RecordInitialTransform(accessory);
    }

    private void OnEnable() {
      foreach (var accessory in Accessories)
        RecordInitialTransform(accessory);
    }

    private void RecordInitialTransform(IAccessory accessory) {
      if (accessory == null) return;
      accessory.InitialLocalPosition = accessory.transform.localPosition;
      accessory.InitialLocalRotation = accessory.transform.localRotation;
      accessory.InitialBuildingPosition = transform.position;
      accessory.InitialBuildingRotation = transform.rotation;
      accessory.LastKnownLocalPosition = accessory.InitialLocalPosition;
      accessory.LastKnownLocalRotation = accessory.InitialLocalRotation;
    }

    private void Update() {
      foreach (var accessory in Accessories) {
        if (accessory == null)
          continue;

        // Detect manual move/rotate in the editor or at runtime
        if (accessory.transform.localPosition != accessory.LastKnownLocalPosition ||
            accessory.transform.localRotation != accessory.LastKnownLocalRotation) {
          accessory.InitialLocalPosition = accessory.transform.localPosition;
          accessory.InitialLocalRotation = accessory.transform.localRotation;
          accessory.InitialBuildingPosition = transform.position;
          accessory.InitialBuildingRotation = transform.rotation;
          accessory.LastKnownLocalPosition = accessory.InitialLocalPosition;
          accessory.LastKnownLocalRotation = accessory.InitialLocalRotation;
        }

        if (accessory.FollowBuildingTransform) {
          // Calculate the delta rotation and position from the building's initial state
          Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(accessory.InitialBuildingRotation);
          Vector3 deltaPosition = transform.position - accessory.InitialBuildingPosition;

          // Apply rotation and position in world space, preserving manual changes
          accessory.transform.position = transform.TransformPoint(accessory.InitialLocalPosition) + deltaPosition;
          accessory.transform.rotation = deltaRotation * (transform.rotation * accessory.InitialLocalRotation);
        }

        // Always update last known for next frame
        accessory.LastKnownLocalPosition = accessory.transform.localPosition;
        accessory.LastKnownLocalRotation = accessory.transform.localRotation;
      }
    }

    public void AddAccessory(GameObject go, bool followBuildingTransform = true) {
      if (go == null) return;
      var accessory = go.GetComponent<IAccessory>();
      if (accessory == null)
        accessory = go.AddComponent<AccessoryBehaviour>();
      accessory.FollowBuildingTransform = followBuildingTransform;
      accessory.GizmoColor = UnityEngine.Random.ColorHSV();
      Accessories.Add(accessory);
      go.transform.SetParent(transform, true);
      RecordInitialTransform(accessory);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
      foreach (var accessory in Accessories) {
        if (accessory != null) {
          Gizmos.color = accessory.GizmoColor;
          Gizmos.DrawWireSphere(accessory.transform.position, 0.2f);
        }
      }
    }

    private void OnTransformChildrenChanged() {
      foreach (Transform child in transform) {
        var accessory = child.GetComponent<IAccessory>();
        if (accessory != null && !Accessories.Contains(accessory)) {
          Accessories.Add(accessory);
          RecordInitialTransform(accessory);
        }
        else if (accessory == null) {
          accessory = child.gameObject.AddComponent<AccessoryBehaviour>();
          Accessories.Add(accessory);
          RecordInitialTransform(accessory);
        }
      }

      Accessories.RemoveAll(a => a == null || a.transform.parent != transform);
    }

    private void OnDrawGizmosSelected() {
      if (_spriteRenderer != null && _spriteRenderer.sprite != null) {
        Gizmos.color = Color.yellow;
        var bounds = _spriteRenderer.bounds;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
      }
    }
#endif
  }
}