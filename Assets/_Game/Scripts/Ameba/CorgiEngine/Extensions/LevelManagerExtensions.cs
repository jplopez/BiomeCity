using Ameba.CorgiEngine;
using Ameba.Runtime;
using MoreMountains.CorgiEngine;

namespace Ameba.CorgiEngine.Extensions {

  public static class LevelManagerExtensions {
    public static void Apply(this LevelManager manager, LevelConfig config) {
      RuntimeConfigApplier<LevelManager,LevelConfig>.ApplyConfig(manager, config);
    }
  }
}