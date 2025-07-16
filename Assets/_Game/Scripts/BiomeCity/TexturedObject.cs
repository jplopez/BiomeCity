using UnityEngine;

namespace BiomeCity {
  public enum MaterialOptions {
    Ground_Gray = 0,
    Blue = 1,
    Green = 2,
    Red = 3,
    Orange = 4,
    Yellow = 5,
  }

  [CreateAssetMenu(fileName = "TexturedObject", menuName = "BiomeCity/Textured Object")]
  public class TexturedObject : ScriptableObject {
    [Header("Dimensions")]
    [Tooltip("The width represented in number of Grids")]
    public int Width = 1;
    [Tooltip("The height represented in number of Grids")]
    public int Height = 1;

    [Header("Materials")]
    [Tooltip("List of possible materials for this object")]
    public MaterialOptions MaterialOption = MaterialOptions.Ground_Gray;

    [Tooltip("Default sprite for the texture")]
    public Sprite DefaultSprite;

    [Tooltip("Default scale factor for the object")]
    [SerializeField]
    protected float _scaleFactor = 1f;

    protected MaterialLibrary _library;

    private void Awake() {
      _library = MaterialLibrary.Instance;
    }
    /// <summary>
    /// Instantiates a new GameObject with a SpriteRenderer using the specified material index.
    /// </summary>
    /// <param name="materialIndex">Index of the material to use from the Materials list.</param>
    /// <returns>The instantiated GameObject.</returns>
    public GameObject InstantiateObject(int materialIndex = 0) {
      var go = new GameObject(name);

      var renderer = go.AddComponent<SpriteRenderer>();
      renderer.sprite = DefaultSprite;

      // Use MaterialLibrary to get the correct material
      Material mat = _library.GetMaterial(MaterialOption);
      if (mat != null)
        renderer.sharedMaterial = mat;

      // Set scale based on Width, Height, and scale factor
      go.transform.localScale = new Vector3(_scaleFactor * Width, _scaleFactor * Height, 1);

      return go;
    }
  }
}