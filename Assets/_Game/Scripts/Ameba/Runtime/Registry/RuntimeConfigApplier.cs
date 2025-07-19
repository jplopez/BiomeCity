using System;
using System.Reflection;
using UnityEngine;

namespace Ameba.Runtime {

  /// <summary>
  /// Utility class to apply a RuntimeConfig instances into a Component
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="Tconfig"></typeparam>
  public static class RuntimeConfigApplier<T, Tconfig>
    where T : Component
    where Tconfig : class, new() {
    public static T ApplyConfig(T component, Tconfig config) {
      Type configType = config.GetType();
      Type componentType = component.GetType();

      var configAttr = configType.GetCustomAttribute<RuntimeConfigAttribute>();
      var defaultCopyMode = configAttr?.DefaultCopyMode ?? CopyMode.Direct;

      foreach (MemberInfo member in configType.GetMembers(BindingFlags.Public | BindingFlags.Instance)) {

        // get ConfigFieldAttribute honoring class-level defaults for implicit members
        var attr = ResolveConfigFieldAttribute(member, configType);
        // Exit use cases
        if (attr == null) continue;
        if (attr.IgnoreOnApply) continue;

        string targetName = attr.TargetFieldName ?? member.Name;
        CopyMode mode = attr.Mode;
        Type strategyType = attr.StrategyType;

        //obtain member value considering Fields and Properties
        object value = null;
        if (member is FieldInfo fi) value = fi.GetValue(config);
        else if (member is PropertyInfo pi && pi.GetIndexParameters().Length == 0) value = pi.GetValue(config);

        // CopyMode.Custom
        if (mode == CopyMode.Custom && strategyType != null) {
          if (Activator.CreateInstance(strategyType) is not IConfigFieldCopier<T> copierInstance) {
            Debug.LogWarning($"[{componentType.Name}] Failed to initialize custom copier '{strategyType.Name}'");
          }
          else {
            copierInstance.Apply(component, value);
          }
          continue;
        }
        
        //Ensure target member exists and is valid
        var targetMembers = componentType.GetMember(targetName, BindingFlags.Public | BindingFlags.Instance);
        var target = targetMembers.Length > 0 ? targetMembers[0] : null;
        if (target == null) {
          Debug.LogWarning($"[{componentType.Name}] Target member '{targetName}' not found for config member '{member.Name}'");
          continue;
        }

        // CopyMode.Direct (assign member value into target's)
        if(mode == CopyMode.Direct) {
          if (target is FieldInfo field) {
            field.SetValue(component, value);
          }
          else if (target is PropertyInfo prop && prop.CanWrite) {
            prop.SetValue(component, value);
          }
        }
        // CopyMode.Instantate (new instance of member's value into target's)
        if (mode == CopyMode.Instantiate && value is UnityEngine.Object obj) {
          var clone = UnityEngine.Object.Instantiate(obj);
          if (target is FieldInfo instField) {
            instField.SetValue(component, clone);
          }
          else if (target is PropertyInfo prop && prop.CanWrite && prop.GetSetMethod() != null) {
            prop.SetValue(component, clone);
          }
        }
#if UNITY_EDITOR
        Debug.Log($"[ApplyConfig] '{targetName}' ← '{value}'");
#endif
      } //foreach

      return component;
    }

    public static Tconfig MergeConfigs(Tconfig baseConfig, Tconfig overrideConfig, bool forceOverride = false) {

      if (!typeof(Tconfig).GetConstructor(Type.EmptyTypes)?.IsPublic ?? true) { 
        Debug.LogError($"MergeConfigs: {typeof(Tconfig).Name} must have a public parameterless constructor. Returning original config object");
        return baseConfig;
      }

      var merged = new Tconfig();
      Type configType = typeof(Tconfig);
      FieldInfo[] fields = configType.GetFields(BindingFlags.Public | BindingFlags.Instance);
      RuntimeConfigAttribute configAttr = configType.GetCustomAttribute<RuntimeConfigAttribute>();
      MergeMode defaultMergeMode = configAttr?.DefaultMergeMode ?? MergeMode.ReplaceIfDefault;

      foreach (FieldInfo field in fields) {
        var baseValue = field.GetValue(baseConfig);
        var overrideValue = field.GetValue(overrideConfig);
        var finalValue = baseValue;

        ConfigFieldAttribute fieldAttr = ResolveConfigFieldAttribute(field, configType);
        MergeMode mergeMode = fieldAttr.MergeMode;

        bool shouldReplace = mergeMode switch {
          MergeMode.AlwaysReplace => true,
          MergeMode.ReplaceIfDefault => !IsValueOverridable(field, baseValue),
          _ => false
        };

        if (forceOverride || shouldReplace) {
          finalValue = overrideValue;
        }

        try {
          field.SetValue(merged, finalValue);
#if UNITY_EDITOR
          Debug.Log($"[MergeConfigs] Field '{field.Name}' | MergeMode: '{mergeMode}' | Value: '{baseValue}' → '{finalValue}'");
#endif
        }
        catch (Exception ex) {
          Debug.LogError($"Merge error on '{field.Name}' : {ex.Message}");
#if UNITY_EDITOR
          throw;
#endif
        }
      }
      return merged;
    }

    private static ConfigFieldAttribute ResolveConfigFieldAttribute(MemberInfo member, Type configType) {
      var configAttr = configType.GetCustomAttribute<RuntimeConfigAttribute>();
      var attr = member.GetCustomAttribute<ConfigFieldAttribute>();

      if (attr == null && configAttr != null) {
        attr = new ConfigFieldAttribute(
            targetFieldName: member.Name,
            mode: configAttr.DefaultCopyMode,
            strategyType: null,
            mergeMode: configAttr.DefaultMergeMode,
            ignoreOnApply: false
        );
      }
      return attr;
    }

    private static bool IsValueOverridable(FieldInfo field, object currentValue) {
      var type = field.FieldType;
      if (type == typeof(string)) return currentValue is not string str || !string.IsNullOrEmpty(str);
      if (typeof(UnityEngine.Object).IsAssignableFrom(type)) return currentValue != null;
      if (type.IsEnum) return false;
      if (type.IsValueType) return !Equals(currentValue, Activator.CreateInstance(type));
      return currentValue != null;
    }
  }

}