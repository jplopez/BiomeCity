using UnityEngine;

namespace Ameba.SSU {
  public class SSUShader : SSUShaderBase {

    [SSUPropertyGroup("Frozen", SSUHeaderColor.Green)]
    [SSUProperty("Fade")]
    public float Fade;

    [SSUProperty("Tint")]
    public Color Tint;

    [SSUProperty("Contrast")]
    public float Contrast;

 
    [SSUPropertyGroup("FrozenSnow", SSUHeaderColor.Orange)]
    [SSUProperty("Color")]
    public Color SnowColor;

    [SSUProperty("Contrast")]
    public float SnowContrast;

    [SSUProperty("Density")]
    public float SnowDensity;

    [SSUProperty("Scale")]
    public Vector2 SnowScale;

  
    [SSUPropertyGroup("FrozenHighlight", SSUHeaderColor.Teal)]
    [SSUProperty("Color")]
    public Color HighlightColor;

    [SSUProperty("Contrast")]
    public float HighlightContrast;

    [SSUProperty("Density")]
    public float HighlightDensity;

    [SSUProperty("Speed")]
    public Vector2 HighlightSpeed;

    [SSUProperty("Scale")]
    public Vector2 HighlightScale;

    [SSUProperty("Distortion")]
    public Vector2 HighlightDistortion;

    [SSUProperty("DistortionSpeed")]
    public Vector2 HighlightDistortionSpeed;

    [SSUProperty("DistortionScale")]
    public Vector2 HighlightDistortionScale;

  }
}