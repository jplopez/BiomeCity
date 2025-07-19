🧠 Configuration Models for RuntimeRegistry<TTarget>

You can define configuration classes in one of two ways: POCOs (Plain Old C# Objects) or ScriptableObject wrappers. Both work seamlessly as long as the configuration type uses [RuntimeConfig(typeof(TTarget))].

💡 Option 1: POCO-Based Configs

Define configuration classes as plain C# types:
[RuntimeConfig(typeof(LevelManager))]
public class LevelConfig
{
    public string SceneName;
    public int SpawnCount;
    public GameObject[] PlayerPrefabs;
}


✅ Pros
- Runtime instantiable via new LevelConfig() or Activator.CreateInstance
- Easy to test and merge
- Great for loading from JSON, YAML, remote APIs, etc.
- No Unity serialization quirks or asset overhead
❌ Cons
- Not directly editable in the Inspector
- Requires custom editors or Unity tooling for visual editing
- Cannot be drag-and-dropped into components or assets

🧩 Option 2: ScriptableObject-Wrapped Configs

Encapsulate your POCO config inside a Unity asset:

[CreateAssetMenu(menuName = "MyGame/Configs/LevelConfig")]
public class LevelConfigAsset : ScriptableObject
{
    [SerializeField] public LevelConfig Config = new LevelConfig();
}


✅ Pros
- Fully editable in Unity Inspector
- Serializable as .asset files for build-time bundling or Addressables
- Designers can create and tweak scene-specific configs easily
❌ Cons
- Limited runtime creation (you must use ScriptableObject.CreateInstance)
- Slightly more boilerplate for wrapping and extracting the payload
- Harder to serialize to JSON without intermediate formats

🔧 Applying the Configuration
Regardless of which format you use, the configuration must be annotated:
[RuntimeConfig(typeof(LevelManager))]


And will be applied automatically by RuntimeRegistryInitializer<LevelManager, MyRegistry> using field-level [ConfigField(...)] annotations.
