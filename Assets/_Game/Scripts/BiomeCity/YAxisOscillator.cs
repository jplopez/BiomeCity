using UnityEngine;

namespace BiomeCity {
  public class YAxisOscillator : MonoBehaviour {
    public float maxAngle = 30f;      // Maximum rotation angle in degrees
    public float frequency = 1f;      // Oscillation Speed

    private Quaternion startRotation;

    void Start() {
      startRotation = transform.rotation;
    }

    void Update() {
      float angle = Mathf.Sin(Time.time * frequency) * maxAngle;
      transform.rotation = startRotation * Quaternion.Euler(0f, angle, 0f);
    }
  }
}