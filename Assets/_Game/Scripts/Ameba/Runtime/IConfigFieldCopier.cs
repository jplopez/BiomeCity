namespace Ameba.Runtime {
  public interface IConfigFieldCopier<TComponent> {
    void Apply(TComponent component, object value);
  }

}