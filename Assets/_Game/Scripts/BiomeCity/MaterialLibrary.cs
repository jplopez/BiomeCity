
using UnityEngine;
using UnityEngine.Timeline;

namespace BiomeCity {
  public class MaterialLibrary : MonoBehaviour {
    public Material[] Materials;

    public static MaterialLibrary Instance { get; internal set; }

    private void Awake() => Instance = this;

    public Material GetMaterial(MaterialOptions option) {
      int index = (int)option;
      if (Materials != null && index >= 0 && index < Materials.Length)
        return Materials[index];
      return null;
    }
  }
}