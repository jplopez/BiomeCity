#if UNITY_EDITOR
using System;

namespace Ameba.Tools {

  [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
  public class InspectorButtonAttribute : Attribute {
    public string ButtonText { get; }

    public InspectorButtonAttribute() { }

    public InspectorButtonAttribute(string buttonText) {
      ButtonText = buttonText;
    }
  }

}
#endif