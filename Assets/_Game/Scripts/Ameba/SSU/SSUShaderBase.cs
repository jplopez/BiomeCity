using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
#endif

namespace Ameba.SSU {

  public enum SSUUpdateType { None, Update, LateUpdate }

  public abstract class SSUShaderBase : MonoBehaviour {

    #region PROPERTIES

    [Tooltip("Material using an SSU shader.")]
    public Material SSUMaterial;

    [Tooltip("When to apply property changes to the material.")]
    public SSUUpdateType UpdateType = SSUUpdateType.None;

    [System.Serializable]
    public class SSUPropertyChangedEvent : UnityEvent<string, object, object> { }

    [Tooltip("Event fired when an SSU property changes.")]
    public SSUPropertyChangedEvent OnSSUPropertyChanged = new();


    // Internal cache of last applied values
    private readonly Dictionary<string, object> lastValues = new();
    private readonly Dictionary<string, bool> lastKeywords = new();


    #endregion

    #region MONOBEHAVIOUR

    protected virtual void Start() {
      InitializeProperties();
      InitializeKeywords();
    }

    protected virtual void Update() {
      if (UpdateType == SSUUpdateType.Update)
        ApplyMaterialChanges();
    }

    protected virtual void LateUpdate() {
      if (UpdateType == SSUUpdateType.LateUpdate)
        ApplyMaterialChanges();
    }

    #endregion

    #region INITIALIZATION


    protected virtual void InitializeProperties() {
      if (SSUMaterial == null) return;

      SSUShaderReflectionUtility.ForEachShaderProperty(this, (fullPropName, field, _) =>
      {
        if (!SSUMaterial.HasProperty(fullPropName)) return;

        object value = null;

        if (field.FieldType == typeof(float))
          value = SSUMaterial.GetFloat(fullPropName);
        else if (field.FieldType == typeof(Color))
          value = SSUMaterial.GetColor(fullPropName);
        else if (field.FieldType == typeof(Vector4))
          value = SSUMaterial.GetVector(fullPropName);
        else if (field.FieldType == typeof(Vector2)) {
          Vector4 v = SSUMaterial.GetVector(fullPropName);
          value = new Vector2(v.x, v.y);
        }
        else if (typeof(Texture).IsAssignableFrom(field.FieldType))
          value = SSUMaterial.GetTexture(fullPropName);

        if (value != null) {
          field.SetValue(this, value);
#if UNITY_EDITOR
          Debug.Log($"[SSUShaderBase] Initialized property '{field.Name}' as {value}");
        } else {
          Debug.Log($"[SSUShaderBase] No value found on shader for property '{field.Name}'");
#endif
        }
      });
    }

    protected virtual void InitializeKeywords() {
      if (SSUMaterial == null) return;

      SSUShaderReflectionUtility.ForEachKeyword(this, (field, attr) =>
      {
        bool value = field.FieldType == typeof(bool)
            ? (bool)field.GetValue(this)
            : attr.InitialState;

        if (value)
          SSUMaterial.EnableKeyword(attr.KeywordName);
        else
          SSUMaterial.DisableKeyword(attr.KeywordName);
#if UNITY_EDITOR
        Debug.Log($"[SSUShaderBase] Initialized keyword '{attr.KeywordName}' as {value}");
#endif
      });
    }

    #endregion

    #region APPLY CHANGES

    protected void ApplyMaterialChanges() {
      if (SSUMaterial == null) return;

      SSUShaderReflectionUtility.ForEachShaderProperty(this, (fullPropName, field, _) => 
      {
        if (!SSUMaterial.HasProperty(fullPropName)) return;

        object value = field.GetValue(this);
        if (!lastValues.TryGetValue(fullPropName, out var previous) || !Equals(previous, value)) {
          ApplyToMaterial(fullPropName, value);
          lastValues[fullPropName] = value;
          OnSSUPropertyChanged.Invoke(fullPropName, previous, value);
        }
      });

      SSUShaderReflectionUtility.ForEachKeyword(this, (field, attr) => {
        bool value = field.FieldType == typeof(bool)
            ? (bool)field.GetValue(this)
            : attr.InitialState;

        bool previous = lastKeywords.TryGetValue(attr.KeywordName, out var pVal) ? pVal : !value;
        if (previous != value) {
          if (value) {
            SSUMaterial.EnableKeyword(attr.KeywordName);
          } else {
            SSUMaterial.DisableKeyword(attr.KeywordName);
          }
          lastKeywords[attr.KeywordName] = value;
        }

      });
    }

    protected virtual void ApplyToMaterial(string propName, object value) {
      switch (value) {
        case bool b:
          ApplyFloat(propName, b ? 1f : 0f);
          break;
        case Color c:
          ApplyColor(propName, c);
          break;
        case Enum e:
          ApplyInt(propName, Convert.ToInt32(e));
          break;
        case float f:
          ApplyFloat(propName, f);
          break;
        case int i:
          ApplyInt(propName, i);
          break;
        case Texture t:
          ApplyTexture(propName, t);
          break;
        case Vector2 v2:
          ApplyVector(propName, v2);
          break;
        case Vector4 v4:
          ApplyVector(propName, v4);
          break;
        default:
          Debug.LogWarning($"[SSUShaderBase] ApplyToMaterial doesn't recognize {value.GetType()}");
          break; 
      }
    }

    protected virtual void ApplyInt(string name, int value) => SSUMaterial.SetInt(name, value);
    protected virtual void ApplyFloat(string name, float value) => SSUMaterial.SetFloat(name, value);
    protected virtual void ApplyColor(string name, Color value) => SSUMaterial.SetColor(name, value);
    protected virtual void ApplyVector(string name, Vector2 value) => SSUMaterial.SetVector(name, new Vector4(value.x, value.y, 0,0));
    protected virtual void ApplyVector(string name, Vector4 value) => SSUMaterial.SetVector(name, value);
    protected virtual void ApplyTexture(string name, Texture value) => SSUMaterial.SetTexture(name, value);

    #endregion
  }

  public static class SSUShaderReflectionUtility {
    public static void ForEachShaderProperty(SSUShaderBase target, Action<string, FieldInfo, SSUPropertyAttribute> callback) {
      string currentPrefix = "";
      bool ignoreGroups = false;

      var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

      foreach (var field in fields) {
        var groupAttr = field.GetCustomAttribute<SSUPropertyGroupAttribute>();
        if (groupAttr != null && !ignoreGroups) {
          currentPrefix = groupAttr.Prefix;
          continue;
        }

        var propAttr = field.GetCustomAttribute<SSUPropertyAttribute>();
        if (propAttr == null) continue;

        if (propAttr.IgnoreGroups) {
          ignoreGroups = true;
          currentPrefix = "";
        }

        string fullPropName = ignoreGroups
            ? $"_{propAttr.ShaderPropertyName}"
            : $"_{currentPrefix}{propAttr.ShaderPropertyName}";

        callback?.Invoke(fullPropName, field, propAttr);
      }
    }

    public static void ForEachKeyword(SSUShaderBase target, Action<FieldInfo, SSUKeywordAttribute> callback) {
      var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

      foreach (var field in fields) {
        var keywordAttr = field.GetCustomAttribute<SSUKeywordAttribute>();
        if (keywordAttr != null) {
          callback?.Invoke(field, keywordAttr);
        }
      }
    }
  }

} // Namespace
