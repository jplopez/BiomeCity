#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Ameba.Tools {
  [CustomEditor(typeof(ProtoBlock))]
  public class ProtoBlockEditor : Editor {
    public override void OnInspectorGUI() {
      DrawDefaultInspector();

      ProtoBlock block = (ProtoBlock)target;
      if (GUILayout.Button("Update Block (Debug)")) {
        block.Initialize();
      }
    }
  }
}
#endif