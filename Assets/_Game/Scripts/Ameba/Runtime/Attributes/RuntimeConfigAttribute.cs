using System;

namespace Ameba.Runtime {

  public enum CopyMode { Direct, Instantiate, Custom }
  public enum MergeMode { ReplaceIfDefault, AlwaysReplace }

  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class RuntimeConfigAttribute : Attribute {
    /// <summary>
    /// The component type this config applies to.
    /// </summary>
    public Type TargetComponentType { get; }

    /// <summary>
    /// Default copy mode to use when applying fields.
    /// </summary>
    public CopyMode DefaultCopyMode { get; }

    /// <summary>
    /// Default merge mode to use when combining configs.
    /// </summary>
    public MergeMode DefaultMergeMode { get; }

    public RuntimeConfigAttribute(
        Type targetType,
        CopyMode defaultCopyMode = CopyMode.Direct,
        MergeMode defaultMergeMode = MergeMode.ReplaceIfDefault) {
      TargetComponentType = targetType;
      DefaultCopyMode = defaultCopyMode;
      DefaultMergeMode = defaultMergeMode;
    }
  }
}