using MoreMountains.CorgiEngine;
using UnityEngine;
using UnityEngine.U2D;

namespace BiomeCity.Platforms {
  public enum PlatformType { Regular, OneWay, MidHeight }
  public enum RendererType { Sprite, SpriteShape, Prefab }


  /// <summary>
  /// Represents a template for defining platform configurations in the BiomeCity game.
  /// </summary>
  /// <remarks>This class is used to specify the type, prefab, and additional behaviors of a platform. It
  /// supports regular platforms, moving platforms, and platforms that appear and disappear.</remarks>
  [CreateAssetMenu(menuName = "BiomeCity/Platform Template")]
  public class PlatformTemplate : ScriptableObject {
    
    //regular, one-way, mid
    public PlatformType PlatformType;

    public GameObject ModelPrefab;

    //Moving Platform
    public bool IsMovingPlatform = false;
    public MovingPlatform MovingPlatformPrefab;

    //AppearDissapear
    public bool IsAppearDisappear = false;
    public AppearDisappear AppearDisappearPrefab;
  }
}