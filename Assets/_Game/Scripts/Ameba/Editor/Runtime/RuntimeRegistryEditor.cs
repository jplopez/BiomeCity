#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEditorInternal;
using UnityEngine;

namespace Ameba.Runtime.Editor {

  [CustomEditor(typeof(RuntimeRegistry<>), true)]
  public class RuntimeRegistryEditor : UnityEditor.Editor {
    private SerializedProperty defaultConfigProp;
    private SerializedProperty configsProp;
    private ReorderableList configsList;
    private Vector2 scrollPosition;
    private int selectedConfigIndex = -1;
    private bool _showDefaultConfig = true;

    private void OnEnable() {
      defaultConfigProp = serializedObject.FindProperty("defaultConfig");
      configsProp = serializedObject.FindProperty("configs");

      if (CheckProperties()) EnableConfigsList();
    }

    protected virtual bool CheckProperties() {
      if (defaultConfigProp == null) {
        Debug.LogWarning("RuntimeRegistryEditor: DefaultConfig property not found."); 
        return false; 
      }
      if (configsProp == null) { 
        Debug.LogWarning("RuntimeRegistryEditor: ConfigsList property not found."); 
        return false; 
      }
      return true;
    }

    protected virtual void EnableConfigsList() {
      configsList = new ReorderableList(serializedObject, configsProp, true, true, true, true) {
        drawHeaderCallback = rect =>
                  EditorGUI.LabelField(rect, "Scene Configs"),

        drawElementCallback = (rect, index, isActive, isFocused) => {
          SerializedProperty element = configsProp.GetArrayElementAtIndex(index);
          rect.y += 2;
          rect.height = EditorGUIUtility.singleLineHeight;

          var sceneNameProp = element.FindPropertyRelative("SceneName");
          if (sceneNameProp == null) {
            Debug.LogWarning($"Scene {index}: Missing SceneName field");
            EditorGUI.LabelField(rect, $"Scene {index}: Missing SceneName field");
          }
          else {
            string sceneName = sceneNameProp.stringValue;
            string propTitle = string.IsNullOrEmpty(sceneName) ? $"Scene {index}" : sceneName;
            EditorGUI.PropertyField(rect, sceneNameProp, new GUIContent(propTitle));
          }
        },

        onSelectCallback = list =>
                  selectedConfigIndex = list.index
      };
    }

    //private void SetupConfigsList() {
    //  configsList = new ReorderableList(serializedObject, configsProp, true, true, true, true) {
    //    drawHeaderCallback = (Rect rect) => {
    //      EditorGUI.LabelField(rect, "Scene Configs");
    //    }
    //  };

    //  configsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
    //    SerializedProperty element = configsList.serializedProperty.GetArrayElementAtIndex(index);
    //    rect.y += 2;
    //    rect.height = EditorGUIUtility.singleLineHeight;

    //    var sceneName = element.FindPropertyRelative("SceneName");
    //    string name = sceneName.stringValue;
    //    EditorGUI.PropertyField(rect, sceneName, new GUIContent(string.IsNullOrEmpty(name) ? $"Scene {index}" : name));
    //  };

    //  configsList.onSelectCallback = list =>
    //              selectedConfigIndex = list.index;
      
    //}

    public override void OnInspectorGUI() {
      serializedObject.Update();

      EditorGUILayout.Space(10);
      DrawDefaultConfig();
      EditorGUILayout.Space(10);
      DrawConfigsList();
      EditorGUILayout.Space(10);

      serializedObject.ApplyModifiedProperties();
    }

    protected virtual void DrawDefaultConfig() {
      _showDefaultConfig = EditorGUILayout.Foldout(_showDefaultConfig, "Default Config", true);
      if (_showDefaultConfig) {

        EditorGUILayout.HelpBox("The default config is used as a fallback for registered configurations at a field basis. This means you can have scene-specific configurations where you only specify the values that are different than the default config", MessageType.Info);

        EditorGUI.indentLevel++;
        DrawConfigFields(defaultConfigProp, true);
        EditorGUI.indentLevel--;
      }
    }

    protected virtual void DrawConfigsList() {
      // Scene Configs Section
      configsList.DoLayoutList();

      if (selectedConfigIndex >= 0 && selectedConfigIndex < configsProp.arraySize) {
        EditorGUILayout.Space(2);
        var selectedConfig = configsProp.GetArrayElementAtIndex(selectedConfigIndex);
        EditorGUILayout.LabelField("Scene Config Override", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        DrawConfigFields(selectedConfig, false);
        EditorGUILayout.EndScrollView();
      }
    }

    protected virtual void DrawConfigFields(SerializedProperty configProp, bool isDefault) {
      var iterator = configProp.Copy();
      var endProp = iterator.GetEndProperty();
      bool enterChildren = true;

      while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProp)) {
        enterChildren = false;

        // skip drawing array size property
        if (iterator.propertyPath.Contains("Array.size")) continue;

        bool isSceneName = iterator.name == "SceneName";
        bool isInherited = IsInherited(iterator, isDefault);

        // fields are drawn with different styles depending of they are inherited or overriden.
        using (new EditorGUI.DisabledScope(isDefault && isSceneName)) {

          //Rect fieldRect = EditorGUILayout.GetControlRect(true);

          var label = iterator.displayName;
          string prefix = isInherited ? "(Inherited)" : "";
  //        EditorGUILayout.BeginHorizontal();
//          EditorGUILayout.LabelField($"{prefix} {label}", isInherited ? GetInheritedLabelStyle() : GetOverrideLabelStyle());
          EditorGUILayout.PropertyField(iterator, new GUIContent($"{prefix} {label}"), true);
    //      EditorGUILayout.EndHorizontal();
        }
      }
    }

    private bool IsInherited(SerializedProperty prop, bool isDefault) {
      if (isDefault || defaultConfigProp == null) return false;

      var defaultField = defaultConfigProp.FindPropertyRelative(prop.name);
      return defaultField != null && SerializedProperty.EqualContents(prop, defaultField);
    }


    private static GUIStyle GetInheritedLabelStyle() {
      var style = new GUIStyle(EditorStyles.label);
      style.normal.textColor = Color.grey;
      return style;
    }

    private static GUIStyle GetOverrideLabelStyle() {
      return new GUIStyle(EditorStyles.label);
      //style.normal.textColor = Color.green;
      //return style;
    }
  }
}