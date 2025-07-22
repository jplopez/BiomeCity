using UnityEngine;

namespace BiomeCity {

  public class VerticalOscillator : MonoBehaviour {

    public float amplitude = 1f;     // How high it goes
    public float frequency = 1f;     // How fast it oscillates

    private Vector3 startPos;

    void Start() {
      startPos = transform.position;
    }

    void Update() {
      float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
      transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);
    }
  }
}