using System.Collections.Generic;
using UnityEngine;

namespace Ameba.Runtime {

  public abstract class AbstractCustomFieldCopier<T> : IConfigFieldCopier<T> {
    public string TargetFieldName;
    protected AbstractCustomFieldCopier(string targetFieldName) {
      TargetFieldName = targetFieldName;
    }

    public virtual void Apply(T component, object value) {
      var field = component.GetType().GetField(TargetFieldName);
      field?.SetValue(component, value);
    }
  }

  public abstract class AbstractListCopier<T, TElement> : AbstractCustomFieldCopier<T> {
 
    protected AbstractListCopier(string targetFieldName) : base(targetFieldName) { }

    public override void Apply(T component, object value) {
      if (value is List<TElement> list) {
        var field = component.GetType().GetField(TargetFieldName);
        field?.SetValue(component, new List<TElement>(list)); // shallow copy
      }
    }
  }

}