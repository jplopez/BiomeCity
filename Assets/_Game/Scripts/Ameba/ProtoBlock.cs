using UnityEngine;

namespace Ameba.Tools {

  [ExecuteAlways]
  [RequireComponent(typeof(SpriteRenderer))]
  [AddComponentMenu("Ameba/Prototype Block")]
  public class ProtoBlock : MonoBehaviour {

    [Header("SpriteRenderer Settings")]
    [Tooltip("Grid size, in pixels")]
    public int GridSize = 250;
    [Tooltip("DrawMode Width in GridSize units")]
    [Min(0.25f)]
    public float Width = 1f;
    [Tooltip("DrawMode Height in GridSize units")]
    [Min(0.25f)]
    public float Height = 1f;
    [Tooltip("Base color of the block")]
    public Color BaseColor = Color.white;
    [Tooltip("Base Sprite")]
    public Sprite BaseSprite;

    [Header("Collider2D Settings")]
    [Tooltip("The Collider2D for the block. BoxCollider2D and EdgeCollider2D are currently supported. This script will attempt to find it in the GameObject")]
    public Collider2D Collider;
    [Tooltip("If true, the collider size will be updated if the sprite size changes")]
    public bool SyncSizeWithSprite = true;

    [Header("EdgeCollider2D Settings")]
    [Tooltip("If the Box has an EdgeCollider2D, this property indicates how far the collider will be from the top edge of the Sprite. Zero = no offset, 0.25 = A quarter of the height. This is nice for collision tolerance or to represent a soft terrain on a platform")]
    [Range(0f, .25f)]
    public float EdgeVerticalOffset = 0.1f;

    public const string DEF_SHADER = "Universal Render Pipeline/2D/Sprite-Lit-Default";
    private SpriteRenderer _spriteRenderer;
    private Shader _defaultShader;
    private bool _initialized = false;

    public void Awake() => Initialize();
    public void OnValidate() => Initialize();

    public void Initialize() {
      // Get or Create SpriteRenderer
      InitializeSpriteRenderer();
      // Get or Create Box
      InitializeCollider();
      _initialized = true;
    }

    protected virtual void InitializeCollider() {
      if (Collider == null && TryGetComponent(out Collider)) {
        Collider.offset = Vector2.zero;
        Collider.enabled = true;
        if (Collider.GetType() != typeof(BoxCollider2D) && Collider.GetType() != typeof(EdgeCollider2D)) {
          Debug.LogWarning("Unsupported Collider2D: " + Collider.GetType() + " . only BoxCollider2D and EdgeCollider2D");
        }
      }
    }

    protected virtual void InitializeSpriteRenderer() {
      if (!TryGetComponent(out _spriteRenderer)) _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
      if (BaseSprite == null) BaseSprite = _spriteRenderer.sprite;
      _spriteRenderer.drawMode = SpriteDrawMode.Tiled;
      _spriteRenderer.tileMode = SpriteTileMode.Continuous;
      // Default Shader
      //_defaultShader = Shader.Find(DEF_SHADER);
      //if (_defaultShader == null) {
      //  Debug.LogWarning("Sprite-Lit-Default shader not found. Make sure URP is installed and set up.");
      //}
      //_spriteRenderer.sharedMaterial.shader = _defaultShader;
    }

    public void Update() {
      if (!_initialized) Initialize();
      UpdateSpriteRenderer();
      UpdateCollider();
    }

    public virtual void UpdateSpriteRenderer() {
      // Update Sprite Renderer with Color, Sprite and DrawMode size
      _spriteRenderer.color = BaseColor;
      _spriteRenderer.sprite = BaseSprite;
      _spriteRenderer.size = CalculateDrawModeSize();
    }

    public virtual void UpdateCollider() {
      // Adjust Collider to match SpriteRenderer size
      if (SyncSizeWithSprite && Collider != null) {
        if (Collider.GetType() == typeof(BoxCollider2D)) {
          ((BoxCollider2D)Collider).size = _spriteRenderer.size;
        }
        if (Collider.GetType() == typeof(EdgeCollider2D)) {
          ((EdgeCollider2D)Collider).points = CalculateEdgeColliderPoints();
          //Vertical offset
          var half = _spriteRenderer.size.y / 2;
          Collider.offset = new Vector2(0f, half - (EdgeVerticalOffset * half));
        }
      }
    }

    protected virtual Vector2[] CalculateEdgeColliderPoints() {
      Vector2[] vector2 = new Vector2[2];
      var half = _spriteRenderer.size.x / 2f;
      vector2[0] = new Vector2(-half, 0f);
      vector2[1] = new Vector2(half, 0f);
      return vector2;
    }

    /// <summary>
    /// Calculates the size of the draw mode based on the current grid size, width, height, 
    /// and the bounds of the base sprite.
    /// </summary>
    /// <returns>A <see cref="Vector2"/> representing the calculated width and height of the draw mode.</returns>
    private Vector2 CalculateDrawModeSize() {
      return new Vector2(Width * (BaseSprite.bounds.size.x * (GridSize / (100f * BaseSprite.bounds.size.x))),
        Height * (BaseSprite.bounds.size.y * (GridSize / (100f * BaseSprite.bounds.size.y))));
    }

  }
}
