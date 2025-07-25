using UnityEngine;

namespace Ameba.SSU {

  public enum SSUHeaderColor {
    Green,
    Yellow,
    LightBlue,
    Red,
    Orange,
    Pink,
    Teal,
    White,
    LightGray
  }


  public class SSUPropertyAttribute : PropertyAttribute {
    public string ShaderPropertyName { get; }
    public bool IgnoreGroups { get; }

    public SSUPropertyAttribute(string shaderPropertyName, bool ignoreGroups = false) {
      ShaderPropertyName = shaderPropertyName;
      IgnoreGroups = ignoreGroups;
    }
  }

  public class SSUPropertyGroupAttribute : PropertyAttribute {
    public string Prefix { get; }
    public SSUHeaderColor HeaderColor { get; }

    public SSUPropertyGroupAttribute(string prefix, SSUHeaderColor headerColor = SSUHeaderColor.LightGray) {
      Prefix = prefix;
      HeaderColor = headerColor;
    }

  }

  public class SSUKeywordAttribute : PropertyAttribute {
    public string KeywordName { get; }
    public bool InitialState { get; }

    public SSUKeywordAttribute(string keywordName, bool initialState = false) {
      KeywordName = keywordName;
      InitialState = initialState;
    }
  }

}