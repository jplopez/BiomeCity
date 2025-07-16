using Ameba.Tools;
using MoreMountains.CorgiEngine;
using UnityEngine;

namespace BiomeCity.Platforms {

  /// <summary>
  /// Controls the behavior and configuration of a platform in the game.
  /// </summary>
  /// <remarks>The <see cref="PlatformController"/> class is responsible for initializing and managing
  /// platform-related components based on the provided <see cref="PlatformTemplate"/>. It dynamically creates and
  /// configures platform elements such as moving platforms and appear/disappear components, ensuring they match the
  /// template's specifications.</remarks>
  public class PlatformController : MonoBehaviour {

    public PlatformTemplate PlatformTemplate;

    private void Start() => LoadTemplate();

    /// <summary>
    /// Create all the inner components of this MonoBehaviour based on the PlatformTemplate
    /// </summary>
#if UNITY_EDITOR
    [InspectorButton("Load Template")]
#endif
    public virtual void LoadTemplate() {
      if (PlatformTemplate == null) return;

      CleanPreviousLoads();

      LoadModelPrefab();

      LoadMovingPlatform();
      LoadAppearDisappear();
    }

    private void LoadModelPrefab() {
      // Instantiate the main platform prefab as a child if provided
      if (PlatformTemplate.ModelPrefab == null) return;

      GameObject modelPrefab = Instantiate(PlatformTemplate.ModelPrefab, transform);
      modelPrefab.name = PlatformTemplate.ModelPrefab.name;

      if (modelPrefab.GetComponent<Collider2D>() == null) {
        if (PlatformTemplate.PlatformType == PlatformType.Regular) {
          //TODO check box collider is present in model, add if not.
          modelPrefab.AddComponent<BoxCollider2D>();
        }
        if (PlatformTemplate.PlatformType == PlatformType.OneWay
          || PlatformTemplate.PlatformType == PlatformType.MidHeight) {
          //TODO check edge collider is present in model, add if not
          modelPrefab.AddComponent<EdgeCollider2D>();
        }
      }
    }

    protected virtual void LoadAppearDisappear() {
      // If AppearDisappear is required, add or copy the component to this GameObject and copy values
      if (PlatformTemplate.IsAppearDisappear && PlatformTemplate.AppearDisappearPrefab != null) {
        var templateComponent = PlatformTemplate.AppearDisappearPrefab.GetComponent<AppearDisappear>();
        AppearDisappear newComponent = null;
        if (templateComponent != null && GetComponent<AppearDisappear>() == null) {
          newComponent = GetComponent<AppearDisappear>();

          // Copy public field values from template to new component (runtime and editor)
          newComponent.Active = templateComponent.Active;
          newComponent.InitialState = templateComponent.InitialState;
          newComponent.StartMode = templateComponent.StartMode;
          newComponent.CyclingMode = templateComponent.CyclingMode;
          newComponent.CyclesAmount = templateComponent.CyclesAmount;
          newComponent.InitialOffset = templateComponent.InitialOffset;
          newComponent.VisibleDuration = templateComponent.VisibleDuration;
          newComponent.HiddenDuration = templateComponent.HiddenDuration;
          newComponent.VisibleToHiddenDuration = templateComponent.VisibleToHiddenDuration;
          newComponent.HiddenToVisibleDuration = templateComponent.HiddenToVisibleDuration;
          newComponent.VisibleFeedback = templateComponent.VisibleFeedback;
          newComponent.VisibleToHiddenFeedback = templateComponent.VisibleToHiddenFeedback;
          newComponent.HiddenFeedback = templateComponent.HiddenFeedback;
          newComponent.HiddenToVisibleFeedback = templateComponent.HiddenToVisibleFeedback;
          newComponent.UpdateAnimator = templateComponent.UpdateAnimator;
          newComponent.EnableDisableCollider = templateComponent.EnableDisableCollider;
          newComponent.ShowHideRenderer = templateComponent.ShowHideRenderer;
          newComponent.TriggerArea = templateComponent.TriggerArea;
          newComponent.PreventAppearWhenCharacterInArea = templateComponent.PreventAppearWhenCharacterInArea;
          newComponent.PreventDisappearWhenCharacterInArea = templateComponent.PreventDisappearWhenCharacterInArea;
          // Copy other relevant fields as needed
        }
      }
    }

    protected virtual void LoadMovingPlatform() {
      // If MovingPlatform is required, add or copy the component to this GameObject and copy values
      if (!PlatformTemplate.IsMovingPlatform
          || PlatformTemplate.MovingPlatformPrefab == null) return;

      var templateComponent = PlatformTemplate.MovingPlatformPrefab.GetComponent<MovingPlatform>();
      MovingPlatform newComponent = null;
      if (templateComponent != null && GetComponent<MovingPlatform>() == null) {
        newComponent = gameObject.AddComponent<MovingPlatform>();
        // Copy public field values from template to new component (runtime and editor)

        //MMPathMovement
        newComponent.CycleOption = templateComponent.CycleOption;
        newComponent.LoopInitialMovementDirection = templateComponent.LoopInitialMovementDirection;
        newComponent.PathElements = templateComponent.PathElements;
        newComponent.AlignmentMode = templateComponent.AlignmentMode;
        newComponent.MovementSpeed = templateComponent.MovementSpeed;
        newComponent.AccelerationType = templateComponent.AccelerationType;
        newComponent.Acceleration = templateComponent.Acceleration;
        newComponent.UpdateMode = templateComponent.UpdateMode;
        newComponent.MinDistanceToGoal = templateComponent.MinDistanceToGoal;
        //MovingPlatform
        newComponent.OnlyMovesWhenPlayerIsColliding = templateComponent.OnlyMovesWhenPlayerIsColliding;
        newComponent.OnlyMovesWhenCharacterIsColliding = templateComponent.OnlyMovesWhenCharacterIsColliding;
        newComponent.ResetPositionWhenPlayerRespawns = templateComponent.ResetPositionWhenPlayerRespawns;
        newComponent.ScriptActivated = templateComponent.ScriptActivated;
        newComponent.StartMovingWhenPlayerIsColliding = templateComponent.StartMovingWhenPlayerIsColliding;
        newComponent.PointReachedFeedback = templateComponent.PointReachedFeedback;
        newComponent.EndReachedFeedback = templateComponent.EndReachedFeedback;
        //Events
        newComponent.OnCharacterEnter = templateComponent.OnCharacterEnter;
        newComponent.OnCharacterExit = templateComponent.OnCharacterExit;
      }
    }

    protected virtual void CleanPreviousLoads() {
      // Remove all child GameObjects (including nested children)
      for (int i = transform.childCount - 1; i >= 0; i--) {
        Transform child = transform.GetChild(i);
#if UNITY_EDITOR
        if (Application.isEditor)
          DestroyImmediate(child.gameObject);
        else
#endif
          Destroy(child.gameObject);
      }
    }

  }
}