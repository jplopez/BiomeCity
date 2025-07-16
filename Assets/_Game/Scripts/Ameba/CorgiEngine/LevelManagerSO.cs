using System;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using static MoreMountains.CorgiEngine.LevelManager;

namespace Ameba.CorgiEngine {

  [Serializable]
  public struct LevelConfig {

    public string SceneName;
    public string LevelName;

    //Instantiate Characters
    public List<Character> PlayablePrefabs;
    public bool AutoAttributePlayerIDs;

    //Characters already in the scene
    public List<Character> SceneCharacters;

    //Checkpoints
    public CheckpointsAxis CheckpointAttributionAxis;
    public CheckpointDirections CheckpointAttributionDirection;
    public CheckPoint CurrentCheckPoint;

    //Points of Entry
    public List<PointOfEntry> PointOfEntries;
    public float IntroFadeDuration;

    //Intro and outro duractions
    public float OutroFadeDuration;
    public int FaderID;
    public MMTweenType FadeTween;
    public float RespawnDelay;
    public bool ResetPointsOnRestart;

    //Level Bounds
    public BoundsModes BoundsMode;
    public Bounds LevelBounds;
    public bool ConvertToColliderBoundsButton;
    public Collider BoundsCollider;
    public Collider2D BoundsCollider2D;

    //Scene Loading
    public MMLoadScene.LoadingSceneModes LoadingSceneMode;
    public string LoadingSceneName;

    //Feedbacks
    public bool SetPlayerAsFeedbackRangeCenter;
  }

  /// <summary>
  /// A Scriptable Object to manage the LevelManager settings for all scenes.
  /// For every scene, you create a LevelConfig, any config not defined there, is automatically taken from the DefaultLevelConfig.
  /// </summary>
  public class LevelManagerSO : ScriptableObject {

    public LevelConfig DefaultLevelConfig;

    public Dictionary<string, LevelConfig> Configs;


  } 
}