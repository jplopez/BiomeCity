using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ameba.SSU {
  [CustomPropertyDrawer(typeof(SSUAnimator))]
  public class SSUAnimatorDrawer : PropertyDrawer {

    public static Dictionary<int,int> colorIndexes=new();

    private enum ValueFieldLayout { Vertical, Horizontal };
    private float previewTime = 0f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      SerializedProperty propName = property.FindPropertyRelative("PropertyName");
      SerializedProperty propType = property.FindPropertyRelative("PropType");
      string animatorHeader = $"{propName?.stringValue} ({propType.enumDisplayNames[propType.enumValueIndex]})";
      property.isExpanded = DrawHeader(animatorHeader, property);
      if (property.isExpanded) {
        DrawExpandedDrawer(property);
        DrawPreview(property);
      }
    }

    #region Drawing

    #region Header
    private bool DrawHeader(string headerLabel, SerializedProperty property) {
      bool isFoldoutOpen = property.isExpanded;

      if (!colorIndexes.TryGetValue(property.GetHashCode(), out int headerColorIndex)) {
        headerColorIndex = Math.Abs(property.propertyPath.GetHashCode()) % Enum.GetValues(typeof(SSUHeaderColor)).Length;
        colorIndexes.Add(property.GetHashCode(), headerColorIndex);
      }
      Color headerColor = GetHeaderColor(headerColorIndex);

      var bgColor = GUI.backgroundColor;
      GUI.backgroundColor = headerColor;

      Rect rect = GUILayoutUtility.GetRect(0f, 22f, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(22f));
      EditorGUI.DrawRect(rect, headerColor);

      Rect toggleRect = new(rect.x + 14, rect.y + 4, 16, 16);
      Rect labelRect = new(rect.x + 19, rect.y, rect.width - 19, rect.height);

      // Label style
      int fontSize = EditorStyles.label.fontSize;
      GUIStyle menuStyle = new(EditorStyles.label) {
        fontStyle = FontStyle.Bold,
        fontSize = fontSize * 2,
        normal = { textColor = Color.black, background = Texture2D.blackTexture },
        onNormal = { textColor = Color.black, background = Texture2D.blackTexture },
      };

      //context menu
      Rect menuButtonRect = new(rect.xMax - 18, rect.y + 2, 16, 16);
      if (GUI.Button(menuButtonRect, new GUIContent("⋮", "More options"), menuStyle)){
        ShowContextMenu(property);
      }
      GUIStyle labelStyle = new(EditorStyles.label) {
        fontStyle = FontStyle.Bold,
        normal = { textColor = Color.black },
        onNormal = { textColor = Color.black }
      };
      
      // Update isExpanded via Foldout
      bool newState = EditorGUI.Foldout( toggleRect, isFoldoutOpen, GUIContent.none, true);
      EditorGUI.LabelField(labelRect, headerLabel, labelStyle);
      GUI.backgroundColor = bgColor;
      GUILayout.Space(5);

      return newState;
    }

    private void ShowContextMenu(SerializedProperty property) {
      GenericMenu menu = new();

      menu.AddItem(new GUIContent("Disable"), false, () => SetBool(property, "IsDisabled", true));
      menu.AddItem(new GUIContent("Reset"), false, () => ResetAnimator(property));
      menu.AddItem(new GUIContent("Duplicate"), false, () => DuplicateAnimator(property));
      menu.AddItem(new GUIContent("Remove"), false, () => RemoveAnimator(property));

      menu.ShowAsContext();
    }

    #endregion

    #region Expanded Drawer

    private void DrawExpandedDrawer(SerializedProperty property) {
      //Box:Animator Settings
      EditorGUILayout.Space(2);

      EditorGUILayout.LabelField($"Animation Settings", EditorStyles.boldLabel);
      EditorGUILayout.BeginVertical("box"); // settings fields
      EditorGUI.indentLevel++;
      DrawSettingsField(property, "Active", "Active");
      DrawSettingsField(property, "PropertyName", "Property Name");
      DrawSettingsField(property, "Curve", "Animator Curve");
      DrawSettingsField(property, "Speed");
      EditorGUI.indentLevel--;
      EditorGUILayout.EndVertical(); // settings fields

      EditorGUILayout.Space(5);

      //Box: Property type and values
      EditorGUILayout.LabelField($"Property Values", EditorStyles.boldLabel);
      EditorGUILayout.BeginVertical("box"); // property fields

      //property type dropdown
      SerializedProperty propType = property.FindPropertyRelative("PropType");
      DrawSettingsField(property, "PropType", "Property Type");

      //property values A and B
      EditorGUI.indentLevel++;
      switch ((SSUPropertyType)propType.enumValueIndex) {
        case SSUPropertyType.Float:
          DrawValueFields(property, "FloatA", "FloatB");
          break;
        case SSUPropertyType.Color:
          DrawValueFields(property, "ColorA", "ColorB");
          break;
        case SSUPropertyType.Vector:
          DrawValueFields(property, "VectorA", "VectorB", layout: ValueFieldLayout.Vertical);
          break;
        case SSUPropertyType.Texture:
          DrawValueFields(property, "TextureA", "TextureB");
          break;
        case SSUPropertyType.Int:
          DrawValueFields(property, "IntA", "IntB");
          break;
      }
      EditorGUI.indentLevel--;

      EditorGUILayout.EndVertical(); //end drawer
    }

    private void DrawSettingsField(SerializedProperty property, string propName, string label = "") {
      if (string.IsNullOrEmpty(propName)) return;
      if (string.IsNullOrEmpty(label)) label = propName;
      var field = property.FindPropertyRelative(propName);
      if (field != null) EditorGUILayout.PropertyField(field, new GUIContent(label));
      //EditorGUILayout.Space(2);
    }

    /// <summary>
    /// Convenience method to draw the A and B values. 
    /// A and B are always placed horizontally. The 'layout' parameter only changes the position between a value and its label. 
    /// </summary>
    /// <param name="property"></param>
    /// <param name="valueA"></param>
    /// <param name="valueB"></param>
    /// <param name="labelA"></param>
    /// <param name="labelB"></param>
    /// <param name="layout"></param>
    private void DrawValueFields(SerializedProperty property, string valueA, string valueB, string labelA = "Start", string labelB = "End", ValueFieldLayout layout = ValueFieldLayout.Horizontal) {
      //input and propertyRelative validations
      if (!string.IsNullOrEmpty(valueA) &&
          !string.IsNullOrEmpty(valueB) &&
          (property.FindPropertyRelative(valueA) is var fieldA) && fieldA != null &&
          (property.FindPropertyRelative(valueB) is var fieldB) && fieldB != null) {
        EditorGUILayout.LabelField("Range values");
        //EditorGUILayout.HelpBox("These values tell the Animator the variation of the property over time. For Int,Float,Vector and Colors, the variation relies on a Lerp function. For textures, the at the half of the time, the value changes from Start to End", MessageType.Info);
        EditorGUILayout.Space(2);
        EditorGUILayout.BeginHorizontal(); //Begin layout

        if (layout.Equals(ValueFieldLayout.Vertical)) {
          DrawVerticalField(fieldA, labelA);
          DrawVerticalField(fieldB, labelB);
        }
        else {
          DrawHorizontalField(fieldA, labelA);
          DrawHorizontalField(fieldB, labelB);
        }

        EditorGUILayout.EndHorizontal(); //end layout
      }
    }

    //convenience method for DrawValueFields called when layout is ValueFieldLayout.Vertical
    private void DrawVerticalField(SerializedProperty field, string label) {
      float defaultLabelWidth = EditorGUIUtility.labelWidth;
      float defaultFieldWidth = EditorGUIUtility.fieldWidth;
      EditorGUILayout.BeginVertical();
      EditorGUILayout.LabelField(label, GUILayout.Width(150));
      EditorGUILayout.Space(2);
      EditorGUI.indentLevel++;
      EditorGUIUtility.labelWidth = defaultLabelWidth / 3f; //60f; // tight labels
      EditorGUIUtility.fieldWidth = defaultFieldWidth  / 3f; //60f; // tight labels
      EditorGUILayout.PropertyField(field, GUIContent.none, true);
      EditorGUI.indentLevel--;
      EditorGUIUtility.labelWidth = defaultLabelWidth;
      EditorGUIUtility.fieldWidth = defaultFieldWidth;
      EditorGUILayout.EndVertical();
    }

    //convenience method for DrawValueFields called when layout is ValueFieldLayout.Vertical
    private void DrawHorizontalField(SerializedProperty field, string label) {
      EditorGUILayout.LabelField(label, GUILayout.Width(50));
      EditorGUILayout.PropertyField(field, GUIContent.none, GUILayout.Width(150));
    }


    #region Draw Preview
    private void DrawPreview(SerializedProperty property) {
      // Safe check: Only access managedReferenceValue if allowed
      if (property.propertyType == SerializedPropertyType.ManagedReference
          && (property.managedReferenceValue as SSUAnimator is var animator)) {


        try {
          object previewValue = animator?.Evaluate(previewTime);
          if (previewValue != null) {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Preview Controls", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box"); // settings fields
            EditorGUI.indentLevel++;
            previewTime = EditorGUILayout.Slider("Time", previewTime, 0f, 1f);
            EditorGUILayout.Space(2);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Calculated Value", previewValue.ToString());
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
          }
        }
        catch (Exception ex) {
          EditorGUILayout.HelpBox($"Preview error: {ex.Message}", MessageType.Warning);
        }
      }
      else {
        Debug.LogWarning($"[SSUAnimatorDrawer] Property '{property.propertyPath}' is not a managed reference. Preview evaluation skipped.");
      }
    }

    protected virtual Color GetHeaderColor(int colorIndex) {
      var color = (SSUHeaderColor)Enum.GetValues(typeof(SSUHeaderColor)).GetValue(colorIndex);
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
    #endregion

    #endregion

    #endregion

    #region ACTIONS
    private void SetBool(SerializedProperty property, string fieldName, bool value) {
      var target = property.FindPropertyRelative(fieldName);
      if (target != null) {
        target.boolValue = value;
        property.serializedObject.ApplyModifiedProperties();
      }
    }

    private void ResetAnimator(SerializedProperty property) {
      property.managedReferenceValue = new SSUAnimator();
    }
    private void DuplicateAnimator(SerializedProperty property) { }
    private void RemoveAnimator(SerializedProperty property) { }

    #endregion

  }
}