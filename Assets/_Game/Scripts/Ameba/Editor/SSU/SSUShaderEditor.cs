#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Ameba.SSU {

  [CustomEditor(typeof(SSUShaderBase), true)]
  public class SSUShaderEditor : Editor {

    private Dictionary<string, bool> foldoutStates = new();

    public override void OnInspectorGUI() {
      SSUShaderBase targetScript = (SSUShaderBase)target;
      SerializedObject so = new(targetScript);
      SerializedProperty materialProp = so.FindProperty("SSUMaterial");
      SerializedProperty updateTypeProp = so.FindProperty("UpdateType");
      SerializedProperty eventProp = so.FindProperty("OnSSUPropertyChanged");


      EditorGUILayout.LabelField("Sync Settings", EditorStyles.boldLabel);
      EditorGUI.indentLevel++;
      EditorGUILayout.Space(5);
      EditorGUILayout.PropertyField(updateTypeProp);
      EditorGUILayout.Space(2);
      EditorGUILayout.PropertyField(eventProp);
      EditorGUI.indentLevel--;
      EditorGUILayout.Space(10);

      EditorGUILayout.LabelField("SSUMaterial Settings", EditorStyles.boldLabel);
      EditorGUI.indentLevel++;
      EditorGUILayout.Space(5);
      EditorGUILayout.PropertyField(materialProp);
      EditorGUILayout.Space(5);
      EditorGUI.indentLevel--;
      if (!ValidTarget(targetScript)) return;

      // Build a lookup of shader property names and types
      Dictionary<string, ShaderUtil.ShaderPropertyType> shaderProps = BuildPropertiesLookup(targetScript);

      // Iterate through fields with SSUPropertyAttribute
      DrawShaderProperties(targetScript, so, shaderProps);

      so.ApplyModifiedProperties();
    }

    protected virtual void DrawShaderProperties(SSUShaderBase targetScript, SerializedObject so, Dictionary<string, ShaderUtil.ShaderPropertyType> shaderProps) {
      string currentPrefix = "";
      bool ignoreGroups = false;
      SSUHeaderColor currentColor = SSUHeaderColor.LightGray;

      var fields = targetScript.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      string foldoutKey = "";

      foreach (var field in fields) {
        var groupAttr = field.GetCustomAttribute<SSUPropertyGroupAttribute>();
        if (groupAttr != null && !ignoreGroups) {
          
          currentPrefix = groupAttr.Prefix;
          currentColor = groupAttr.HeaderColor;
          foldoutKey = currentPrefix;

          if (!foldoutStates.ContainsKey(foldoutKey))
            foldoutStates[foldoutKey] = true;

          GUILayout.Space(10);
          bool isFoldoutOpen = foldoutStates[currentPrefix];
          Color headerColor = GetHeaderColor(currentColor);
          var bgColor = GUI.backgroundColor;
          GUI.backgroundColor = headerColor;

          Rect rect = GUILayoutUtility.GetRect(0f, 22f, GUILayout.ExpandWidth(true));
          EditorGUI.DrawRect(rect, headerColor);

          Rect labelRect = new(rect.x + 20, rect.y, rect.width - 20, rect.height);
          Rect toggleRect = new(rect.x , rect.y + 4, 16, 16);

          // Toggle arrow manually
          isFoldoutOpen = EditorGUI.Foldout(toggleRect, isFoldoutOpen, GUIContent.none, true);

          // Label text in black
          GUIStyle labelStyle = new(EditorStyles.label) {
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.black },
            onNormal = { textColor = Color.black }
          };

          EditorGUI.LabelField(labelRect, currentPrefix, labelStyle);
          foldoutStates[currentPrefix] = isFoldoutOpen;
          GUI.backgroundColor = bgColor;
          GUILayout.Space(5);
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

        SerializedProperty prop = so.FindProperty(field.Name);
        if (prop == null) continue;

        bool valid = shaderProps.TryGetValue(fullPropName, out var shaderType);
        bool typeMatches = valid && IsTypeCompatible(prop, shaderType);
        EditorGUI.indentLevel++;
        if (ignoreGroups || foldoutStates.TryGetValue(foldoutKey, out bool isOpen) && isOpen) {
          using (new EditorGUI.DisabledScope(!typeMatches)) {
            EditorGUILayout.PropertyField(prop, new GUIContent(field.Name, fullPropName));
            if (!typeMatches)
              EditorGUILayout.HelpBox($"Type mismatch: {field.FieldType.Name} vs {shaderType}", MessageType.Warning);
          }
          GUILayout.Space(2);
        }
        EditorGUI.indentLevel--;
      }
    }

    protected virtual string ResolveFullPropName(ref string currentPrefix, SSUPropertyAttribute propAttr, bool ignoreGroups) {
      // In the loop:
      if (propAttr.IgnoreGroups) {
        ignoreGroups = true;
        currentPrefix = ""; // reset current group
      }
      return ignoreGroups
        ? $"_{propAttr.ShaderPropertyName}"
        : $"_{currentPrefix}{propAttr.ShaderPropertyName}";
    }

    protected virtual Dictionary<string, ShaderUtil.ShaderPropertyType> BuildPropertiesLookup(SSUShaderBase targetScript) {
      Shader shader = targetScript.SSUMaterial.shader;
      int count = ShaderUtil.GetPropertyCount(shader);

      var shaderProps = new Dictionary<string, ShaderUtil.ShaderPropertyType>();
      for (int i = 0; i < count; i++) {
        string name = ShaderUtil.GetPropertyName(shader, i);
        ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(shader, i);
        shaderProps[name] = type;
      }
      return shaderProps;
    }

    protected virtual bool ValidTarget(SSUShaderBase targetScript) => targetScript.SSUMaterial != null && targetScript.SSUMaterial.shader != null;

    protected virtual bool IsTypeCompatible(SerializedProperty prop, ShaderUtil.ShaderPropertyType shaderType) {
      return shaderType switch {
        ShaderUtil.ShaderPropertyType.Color => prop.propertyType == SerializedPropertyType.Color,
        ShaderUtil.ShaderPropertyType.Vector => prop.propertyType == SerializedPropertyType.Vector2,
        ShaderUtil.ShaderPropertyType.Float or ShaderUtil.ShaderPropertyType.Range => prop.propertyType == SerializedPropertyType.Float,
        ShaderUtil.ShaderPropertyType.TexEnv => prop.propertyType == SerializedPropertyType.ObjectReference,
        _ => false
      };
    }

    protected virtual Color GetHeaderColor(SSUHeaderColor color) {
      return color switch {
        SSUHeaderColor.Green => new Color(0.6f, 1f, 0.6f),
        SSUHeaderColor.Yellow => new Color(1f, 1f, 0.6f),
        SSUHeaderColor.LightBlue => new Color(0.6f, 0.9f, 1f),
        SSUHeaderColor.Red => new Color(1f, 0.6f, 0.6f),
        SSUHeaderColor.Orange => new Color(1f, 0.8f, 0.5f),
        SSUHeaderColor.Pink => new Color(1f, 0.7f, 0.9f),
        SSUHeaderColor.Teal => new Color(0.6f, 1f, 0.9f),
        SSUHeaderColor.White => Color.white,
        SSUHeaderColor.LightGray => new Color(0.9f, 0.9f, 0.9f),
        _ => Color.gray
      };
    }
  }

}
#endif