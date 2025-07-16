// This script should be attached to each prefab instance
using UnityEngine;

namespace Ameba.Materials {


  public class MaterialTilingOffset : MonoBehaviour {

    public Material SharedMaterial;

    public Vector2 Tiling = Vector2.one;
    public Vector2 Offset = Vector2.zero;

    private void OnValidate() => Initialize();
    private void Awake() => Initialize();

    protected virtual void Initialize() {
      if (SharedMaterial == null) return;
      SharedMaterial.mainTextureScale = Tiling;
      SharedMaterial.mainTextureOffset = Offset;
      if (TryGetComponent(out SpriteRenderer _renderer)) {
        _renderer.sharedMaterial = new Material(SharedMaterial);
      }

    }
  }
}