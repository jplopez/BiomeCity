using System;

namespace Ameba.Tools {

  [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
  public class ActionButtonAttribute : Attribute {

    public string ButtonText { get; }
    public ActionButtonAttribute() { }
    public ActionButtonAttribute(string buttonText) {
      this.ButtonText = buttonText;
    }
  }
}