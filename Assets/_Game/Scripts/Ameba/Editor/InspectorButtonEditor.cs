#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Ameba.Tools {

  [CustomEditor(typeof(MonoBehaviour), true)]
  public class InspectorButtonEditor : Editor {
    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      IEnumerable<MethodInfo> methods = target.GetType()
          .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
          .Where(m => m.GetCustomAttributes(typeof(InspectorButtonAttribute), true).Length > 0);
      //Debug.Log("methods " + methods.Count());
      foreach (var method in methods) {
        var attr = (InspectorButtonAttribute)method.GetCustomAttributes(typeof(InspectorButtonAttribute), true).FirstOrDefault();
        string buttonText = attr?.ButtonText ?? ObjectNames.NicifyVariableName(method.Name);

        if (GUILayout.Button(buttonText)) {
          //Debug.Log("calling " + method.Name);
          method.Invoke(target, null);
        }
      }
    }
  }
}