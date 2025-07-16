#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Ameba.Tools {

#if UNITY_EDITOR
  public class ActionButtonEditor : Editor {

    [CustomEditor(typeof(MonoBehaviour), true)]
    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      var methods = target.GetType()
          .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
          .Where(m => m.GetCustomAttributes(typeof(ActionButtonAttribute), true).Length > 0);

      foreach (var method in methods) {
        var attr = (ActionButtonAttribute)method.GetCustomAttributes(typeof(ActionButtonAttribute), true).FirstOrDefault();
        string buttonText = attr?.ButtonText ?? ObjectNames.NicifyVariableName(method.Name);

        if (GUILayout.Button(buttonText)) {
          method.Invoke(target, null);
        }
      } //foreach
    } // OnInspectorGUI
  } // class
#endif
} // namespace