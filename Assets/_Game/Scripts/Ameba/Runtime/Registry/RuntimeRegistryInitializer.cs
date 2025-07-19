using UnityEngine;

namespace Ameba.Runtime {

  public abstract class RuntimeRegistryInitializer<TManager, R, T> : MonoBehaviour
    where TManager : Component
    where R : RuntimeRegistry<T>
    where T : class, new() {

    [SerializeField]
    protected R Registry;

#if UNITY_EDITOR
    private void OnValidate() {
      EnsureRegistry();
    }

    [ContextMenu("Apply Config Preview")]
    private void ApplyPreview() {
      if (Application.isPlaying) return;
      Initialize();
    }
#endif

    void Awake() => Initialize();

    protected virtual void Initialize() {
      if (!EnsureRegistry()) return;

      string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
      if (Registry.TryGetConfigForScene(sceneName, out T config)) {
        TManager manager = FindFirstObjectByType<TManager>() ?? gameObject.AddComponent<TManager>();
        ApplyConfig(manager, config);
        Debug.Log($"[{GetType().Name}] Applied config for scene: {sceneName}");
      }
    }

    protected virtual TManager ApplyConfig(TManager manager, T config) => RuntimeConfigApplier<TManager,T>.ApplyConfig(manager, config);

    private bool EnsureRegistry() {
      if (Registry == null) {
        Debug.LogWarning($"{GetType().Name} has no assigned registry in the Inspector.");
        return false;
      }
      return true;
    }
  }

}