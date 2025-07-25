using UnityEditor;
using UnityEngine;

namespace Ameba.SSU {
  [CustomEditor(typeof(SSUPlayer))]
  public class SSUPlayerEditor : Editor {

    private SerializedProperty animatorListProp;

    //private ReorderableList animatorList;

    void OnEnable() {
      animatorListProp = serializedObject.FindProperty("Animators");

      //animatorList = new ReorderableList(serializedObject, animatorListProp, true, false, false, false) {
      //  drawElementCallback = (rect, index, isActive, isFocused) => {
      //    var element = animatorListProp.GetArrayElementAtIndex(index);
      //    var propertyNameProp = element.FindPropertyRelative("PropertyName");
      //    string label = $"SSUAnimator: " + (propertyNameProp != null ? propertyNameProp.stringValue : "");
      //    EditorGUI.PropertyField(rect, element, new GUIContent(label));
      //  },
      //};
    }

    public override void OnInspectorGUI() {
      serializedObject.Update();

      EditorGUILayout.LabelField("SSU Player", EditorStyles.boldLabel);
      EditorGUILayout.Space(5);
      DrawPlayerSettings();
      EditorGUILayout.Space(10);

      DrawAnimators();
      EditorGUILayout.Space(10);
      DrawActionButtons();

      serializedObject.ApplyModifiedProperties();
    }

    private void DrawPlayerSettings() {
      // Draw core SSUPlayer settings (if any)
      EditorGUILayout.LabelField("Player Settings", EditorStyles.boldLabel);
      EditorGUILayout.Space(2);
      EditorGUI.indentLevel++;
      EditorGUILayout.PropertyField(serializedObject.FindProperty("StartMode"));
      EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayOrder"));
      EditorGUILayout.PropertyField(serializedObject.FindProperty("InitialDelay"));

      var playModeProp = serializedObject.FindProperty("PlayMode");
      EditorGUILayout.PropertyField(playModeProp);
      if(playModeProp.enumValueIndex == (int)SSUPlayerPlayMode.BackAndForth 
        || playModeProp.enumValueIndex == (int)SSUPlayerPlayMode.Looped) {
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayForever"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("NumberOfPlays"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("DelayBetweenPlays"));
        EditorGUI.indentLevel--;
      }

      EditorGUI.indentLevel--;
      EditorGUILayout.LabelField("Material", EditorStyles.boldLabel);
      EditorGUILayout.Space(2);
      EditorGUI.indentLevel++;
      EditorGUI.BeginDisabledGroup(true);
      EditorGUILayout.PropertyField(serializedObject.FindProperty("Renderer"));
      EditorGUI.EndDisabledGroup();
      EditorGUI.indentLevel--;
    }

    private void DrawAnimators() {
      //animatorList.DoLayoutList();
      EditorGUILayout.LabelField("SSU Animators", EditorStyles.boldLabel);
      // Draw each SSUAnimator in the list using its CustomPropertyDrawer
      for (int i = 0; i < animatorListProp.arraySize; i++) {
        var element = animatorListProp.GetArrayElementAtIndex(i);
        EditorGUILayout.PropertyField(element, true); // invokes SSUAnimatorDrawer
      }
    }

    private void DrawActionButtons() {
      EditorGUILayout.BeginVertical();

      if (GUILayout.Button("Add New SSUAnimator")) {
        AddAnimatorInstance((SSUPlayer)target);
      }

      EditorGUILayout.BeginHorizontal();
      SSUPlayer player = (SSUPlayer)target;
      if (player.IsPlaying) {
        if (GUILayout.Button("Stop Animations")) {
          StopAnimations((SSUPlayer)target);
        }
      }
      else {
        if (GUILayout.Button("Play Animations")) {
          PlayAnimations((SSUPlayer)target);
        }
      }
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.EndVertical();
    }

    private void AddAnimatorInstance(SSUPlayer player) {
      var newAnimator = new SSUAnimator(); // Could use specific subclasses later
      player.Animators.Add(newAnimator);
      EditorUtility.SetDirty(player);     // ensure Unity saves the change
    }

    private void PlayAnimations(SSUPlayer player) {
      player.Play();
      EditorUtility.SetDirty(player);
    }

    private void StopAnimations(SSUPlayer player) {
      player.Stop();
      EditorUtility.SetDirty(player);
    }

  }
}