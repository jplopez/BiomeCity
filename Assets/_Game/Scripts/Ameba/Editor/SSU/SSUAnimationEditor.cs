#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Ameba.SSU {

  //[CustomEditor(typeof(SSUAnimator))]
  //public class SSUAnimatorEditor : Editor {

  //  private SerializedProperty propName;
  //  private SerializedProperty propCurve;
  //  private SerializedProperty propSpeed;
  //  private SerializedProperty propType;

  //  private void OnEnable() {

  //    propName = serializedObject.FindProperty("PropertyName");
  //    propCurve = serializedObject.FindProperty("Curve");
  //    propSpeed = serializedObject.FindProperty("Speed");
  //    propType = serializedObject.FindProperty("PropType");
  //  }

  //  public override void OnInspectorGUI() {
  //    serializedObject.Update();

  //    EditorGUILayout.LabelField("SSU Animator", EditorStyles.boldLabel);
  //    EditorGUILayout.Space(2);
  //    EditorGUILayout.BeginVertical("box");
  //    EditorGUILayout.PropertyField(propName);
  //    EditorGUILayout.CurveField(propCurve.animationCurveValue);
  //    EditorGUILayout.FloatField(propSpeed.floatValue);
  //    EditorGUILayout.Space(2);
  //    EditorGUILayout.EnumPopup((SSUPropertyType)propType.enumValueIndex);

  //    switch ((SSUPropertyType)propType.enumValueIndex) {
  //      case SSUPropertyType.Float:
  //        EditorGUILayout.PropertyField(serializedObject.FindProperty("FloatA"));
  //        EditorGUILayout.PropertyField(serializedObject.FindProperty("FloatB"));
  //        break;
  //      case SSUPropertyType.Color:
  //        EditorGUILayout.PropertyField(serializedObject.FindProperty("ColorA"));
  //        EditorGUILayout.PropertyField(serializedObject.FindProperty("ColorB"));
  //        break;
  //      case SSUPropertyType.Vector:
  //        EditorGUILayout.PropertyField(serializedObject.FindProperty("VectorA"));
  //        EditorGUILayout.PropertyField(serializedObject.FindProperty("VectorB"));
  //        break;
  //      case SSUPropertyType.Texture:
  //        EditorGUILayout.PropertyField(serializedObject.FindProperty("TextureA"));
  //        EditorGUILayout.PropertyField(serializedObject.FindProperty("TextureB"));
  //        break;
  //      case SSUPropertyType.Int:
  //        EditorGUILayout.PropertyField(serializedObject.FindProperty("IntA"));
  //        EditorGUILayout.PropertyField(serializedObject.FindProperty("IntB"));
  //        break;
  //    }
  //    EditorGUILayout.EndVertical();

  //    serializedObject.ApplyModifiedProperties();
  //  }
  //}
}
#endif