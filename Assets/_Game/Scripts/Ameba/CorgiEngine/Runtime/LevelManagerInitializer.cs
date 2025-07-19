
using Ameba.Runtime;
using MoreMountains.CorgiEngine;

namespace Ameba.CorgiEngine {
  public class LevelManagerInitializer : RuntimeRegistryInitializer<LevelManager, LevelManagerRegistry, LevelConfig> {
    protected override LevelManager ApplyConfig(LevelManager manager, LevelConfig config) =>
        RuntimeConfigApplier<LevelManager, LevelConfig>.ApplyConfig(manager, config);
    
  }
}