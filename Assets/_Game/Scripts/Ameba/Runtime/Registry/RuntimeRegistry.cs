using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ameba.Runtime {

  public abstract class RuntimeRegistry<T> : ScriptableObject where T : class, new() {
    [SerializeField] protected T defaultConfig;
    [SerializeField] protected List<T> configs = new();

    public T DefaultConfig => defaultConfig;
    public List<T> Configs => configs;

    public virtual T GetConfigForScene(string sceneName) =>
        configs.FirstOrDefault(c => GetSceneName(c) == sceneName) ?? defaultConfig;

    public virtual bool TryGetConfigForScene(string sceneName, out T config) {
      config = GetConfigForScene(sceneName);
      return config.Equals(defaultConfig);
    }

    protected virtual string GetSceneName(T config) =>
        config.GetType().GetField("SceneName")?.GetValue(config) as string;

  }
}