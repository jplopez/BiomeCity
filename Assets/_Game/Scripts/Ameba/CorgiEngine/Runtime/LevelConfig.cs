using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using UnityEngine;

using Ameba.Runtime;

using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using static MoreMountains.CorgiEngine.LevelManager;

namespace Ameba.CorgiEngine {
  [Serializable]
  [RuntimeConfig(typeof(LevelManager), CopyMode.Direct, MergeMode.ReplaceIfDefault )]
  public class LevelConfig {

    public string SceneName;

    //Instantiate Characters
    [Header("Characters")]
    [AllowNull] public Character[] PlayerPrefabs;
    [AllowNull] public bool AutoAttributePlayerIDs;

    //Characters already in the scene
    [AllowNull] public List<Character> SceneCharacters;

    //Checkpoints
    [Header("Checkpoints")]
    [AllowNull] public CheckPoint DebugSpawn;
    [AllowNull] public CheckpointsAxis CheckpointAttributionAxis;
    [AllowNull] public CheckpointDirections CheckpointAttributionDirection;

    //Points of Entry
    [Header("Points of Entry")]
    [AllowNull] public List<PointOfEntry> PointsOfEntry;

    //Intro and outro duractions
    [Header("Intro and Outro")]
    [AllowNull] public float IntroFadeDuration;
    [AllowNull] public float OutroFadeDuration;
    [AllowNull] public int FaderID;
    [AllowNull] public MMTweenType FadeTween;

    [Header("Respawn")]
    [AllowNull] public float RespawnDelay;
    [AllowNull] public bool ResetPointsOnRestart;

    //Level Bounds
    [Header("Level Bounds")]
    [AllowNull] public BoundsModes BoundsMode;
    [AllowNull] public Bounds LevelBounds;
    [AllowNull] public Collider BoundsCollider;
    [AllowNull] public Collider2D BoundsCollider2D;

    //Scene Loading
    [Header("Scene Loading")]
    [AllowNull] public MMLoadScene.LoadingSceneModes LoadingSceneMode;
    [AllowNull] public string LoadingSceneName;

    //Feedbacks
    [Header("Feedbacks")]
    [AllowNull] public bool SetPlayerAsFeedbackRangeCenter;

    public LevelConfig() {
      SceneName = "";

      //Inferred Default values
      PlayerPrefabs = null;
      SceneCharacters = new();
      DebugSpawn = null;
      PointsOfEntry = new();
      BoundsCollider = null;
      BoundsCollider2D = null;

      //Default values from CogiEngine.LevelManager
      AutoAttributePlayerIDs = false;
      CheckpointAttributionAxis = CheckpointsAxis.x;
      CheckpointAttributionDirection = CheckpointDirections.Ascending;
      IntroFadeDuration = 1f;
      OutroFadeDuration = 1f;
      FaderID = 0;
      FadeTween = new MMTweenType(MMTween.MMTweenCurve.EaseInCubic);
      RespawnDelay = 2f;
      ResetPointsOnRestart = true;
      BoundsMode = BoundsModes.TwoD;
      LevelBounds = new Bounds(Vector3.zero, Vector3.one * 10);
      LoadingSceneMode = MMLoadScene.LoadingSceneModes.MMSceneLoadingManager;
      LoadingSceneName = "LoadingScreen";
      SetPlayerAsFeedbackRangeCenter = false;
    }

  }

}