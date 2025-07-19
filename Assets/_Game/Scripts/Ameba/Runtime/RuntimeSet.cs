using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ameba.Runtime {

  public abstract class RuntimeSet<T> : ScriptableObject {

    protected List<T> _items = new();

    public virtual void Initialize() => _items.Clear();

    public virtual T GetItem(int index) => _items[index];

    public virtual T FirstOrDefault() => _items.Count == 0 ? default : _items.First();

    public virtual void Add(T thing) { if(!_items.Contains(thing)) _items.Add(thing); }
    
    public virtual void Remove(T thing) { if (_items.Contains(thing)) _items.Remove(thing); }
    
    public virtual bool Contains(T thing) => _items.Contains(thing);

  }
}