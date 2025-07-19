using System;

namespace Ameba.Runtime {

 
  /// <summary>
  /// Define a field to be used by runtime config. 
  /// CopyMode : how to copy the field
  /// StrategyType : if CopyMode is Custom, this is the class implementing the copier
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class ConfigFieldAttribute : Attribute {
    /// <summary>
    /// Target field or property name on the component that should receive the config value.
    /// </summary>
    public string TargetFieldName { get; }

    /// <summary>
    /// Defines how the value is copied during ApplyConfig.
    /// </summary>
    public CopyMode Mode { get; }

    /// <summary>
    /// Optional strategy for custom copy behavior.
    /// </summary>
    public Type StrategyType { get; }

    /// <summary>
    /// Defines how the value is merged when combining default + scene configs.
    /// </summary>
    public MergeMode MergeMode { get; }

    /// <summary>
    /// If true, this field is ignored during ApplyConfig but still included in MergeConfigs.
    /// </summary>
    public bool IgnoreOnApply { get; }


    public ConfigFieldAttribute(
        string targetFieldName,
        CopyMode mode = CopyMode.Direct,
        Type strategyType = null,
        MergeMode mergeMode = MergeMode.ReplaceIfDefault,
        bool ignoreOnApply = false) {
      TargetFieldName = targetFieldName;
      Mode = mode;
      StrategyType = strategyType;
      MergeMode = mergeMode;
      IgnoreOnApply = ignoreOnApply;
    }
  }

  /// <summary>
  /// Marks a field to be ignored by a Runtime config
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class IgnoreConfigFieldAttribute : Attribute { }
  

}