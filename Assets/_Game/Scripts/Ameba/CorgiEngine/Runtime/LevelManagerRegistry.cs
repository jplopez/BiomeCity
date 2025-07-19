using MoreMountains.CorgiEngine;
using System.Collections.Generic;
using UnityEngine;
using Ameba.Runtime;

namespace Ameba.CorgiEngine {

  /// <summary>
  /// A Scriptable Object to manage the LevelManager settings for all scenes.
  /// For every scene, you create a LevelConfig, any config not defined there, is automatically taken from the DefaultConfig.
  /// </summary>
  [CreateAssetMenu(fileName = "LevelManagerRegistry", menuName = "Ameba/CorgiEngine/LevelManager Registry")]
  public class LevelManagerRegistry : RuntimeRegistry<LevelConfig> { }


}