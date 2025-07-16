using UnityEditor;
using UnityEngine;
using BiomeCity.Platforms;
using MoreMountains.CorgiEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(PlatformTemplate))]
public class PlatformTemplateEditor : Editor {

  SerializedProperty platformTypeProp;
  SerializedProperty platformPrefab;

  // Platform Extensions

  //Moving
  SerializedProperty isMovingProp;
  SerializedProperty movingPlatformPrefab;
  //Appear Dissapear
  SerializedProperty isAppearDisappear;
  SerializedProperty appearDisappearPrefab;

  //Selected
  MovingPlatform selectedMovingPlatform;
  AppearDisappear selectedAppearDissapear;

  public void OnEnable() {
    platformTypeProp = serializedObject.FindProperty("PlatformType");
    platformPrefab = serializedObject.FindProperty("ModelPrefab");

    isMovingProp = serializedObject.FindProperty("IsMovingPlatform");
    movingPlatformPrefab = serializedObject.FindProperty("MovingPlatformPrefab");

    isAppearDisappear = serializedObject.FindProperty("IsAppearDisappear");
    appearDisappearPrefab = serializedObject.FindProperty("AppearDisappearPrefab");

    //// Add custom prefabs fields if not present
    //var so = (PlatformTemplate)target;
    //var soType = so.GetType();
    //if (soType.GetField("MovingPlatformPrefab") == null)
    //  soType.GetField("MovingPlatformPrefab", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
    //if (soType.GetField("ModelPrefab") == null)
    //  soType.GetField("ModelPrefab", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

    // Use SerializedObject extensions for custom scripts

  }

  public override void OnInspectorGUI() {
    serializedObject.Update();
    EditorGUILayout.PropertyField(platformTypeProp);
    // --- Renderer Section ---
    PlatformPrefabGUI();
    // --- Draw Platform Type Section 
    PlatformExtGUI();
    // Draw other properties
    DrawPropertiesExcluding(serializedObject, "m_Script",
        "PlatformType", "ModelPrefab",
        "IsMovingPlatform", "MovingPlatformPrefab",
        "IsAppearDisappear", "AppearDisappearPrefab");
    serializedObject.ApplyModifiedProperties();
  }

  private void PlatformPrefabGUI() {
    EditorGUILayout.Space();
    EditorGUILayout.LabelField("Platform Settings", EditorStyles.boldLabel);
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(platformPrefab, new GUIContent("Prefab"));
    EditorGUILayout.EndHorizontal();
  }

  private void PlatformExtGUI() {
    EditorGUILayout.Space();
    EditorGUILayout.LabelField("Platform Extensions", EditorStyles.boldLabel);
    EditorGUILayout.Space();


    // Check if IsMovingPlatform property exists and is true
    EditorGUILayout.PropertyField(isMovingProp, new GUIContent("Moving Platform?"));
    if (isMovingProp != null && isMovingProp.boolValue) {
      EditorGUI.indentLevel++;
      //EditorGUILayout.Space();
      EditorGUILayout.HelpBox("Provide a CorgiEngine's MovingPlatform prefab", MessageType.Info);
      EditorGUILayout.BeginHorizontal();
      if (movingPlatformPrefab != null) {
        selectedMovingPlatform = (MovingPlatform)movingPlatformPrefab.objectReferenceValue;
      }
      EditorGUILayout.LabelField("MovingPlatform Prefab", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
      selectedMovingPlatform = (MovingPlatform)EditorGUILayout.ObjectField(selectedMovingPlatform, typeof(MovingPlatform), false);

      if (selectedMovingPlatform != null) {
        movingPlatformPrefab.objectReferenceValue = selectedMovingPlatform;
      }
      EditorGUILayout.EndHorizontal();
      EditorGUI.indentLevel--;
    }
    EditorGUILayout.Space();

    // Check if AppearDissapear
    EditorGUILayout.PropertyField(isAppearDisappear, new GUIContent("Appear Disappear?"));
    if (isAppearDisappear != null && isAppearDisappear.boolValue) {
      EditorGUI.indentLevel++;
      EditorGUILayout.HelpBox("Provide a CorgiEngine's AppearDisappear prefab", MessageType.Info);

      EditorGUILayout.BeginHorizontal();
      if (appearDisappearPrefab != null) {
        selectedAppearDissapear = (AppearDisappear)appearDisappearPrefab.objectReferenceValue;
      }
      EditorGUILayout.LabelField("AppearDisappear Prefab", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
      selectedAppearDissapear = (AppearDisappear)EditorGUILayout.ObjectField(selectedAppearDissapear, typeof(AppearDisappear), false);

      if (selectedAppearDissapear != null) {
        appearDisappearPrefab.objectReferenceValue = selectedAppearDissapear;
      }
      EditorGUILayout.EndHorizontal();
      EditorGUI.indentLevel--;
    }
    EditorGUILayout.Space();


  }
}
#endif