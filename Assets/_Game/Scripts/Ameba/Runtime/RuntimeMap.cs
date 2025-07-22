using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ameba.Runtime {
  public abstract class RuntimeMap<K,V> : ScriptableObject {

    //Helper struct to emulate a KeyValue structure
    protected struct KeyValue {
      public K key;
      public V value;

      public KeyValue(K key, V value) {
        this.key = key;
        this.value = value;
      }
      public bool Equals(KeyValue other) => !other.Equals(default) && key.Equals(other.key) && value.Equals(other.value);
    }

    protected List<KeyValue> _items = new();

    protected virtual void AddKeyValue(KeyValue newKv) {
      // prevent duplicates based on Key
      if (!_items.Any(kv => kv.key.Equals(newKv.key))) {
        _items.Add(newKv);
      }
    }

    protected virtual bool RemoveByKey(K key) {
      var match = FirstOrDefault(key);
      if (!match.Equals(default)) {
        return _items.Remove(match);
      }
      return false;
    }

    protected virtual KeyValue FirstOrDefault(K key) => _items.FirstOrDefault(kv => kv.key.Equals(key));

    public virtual void Initialize() => _items.Clear();

    public virtual V GetItem(K key) {
      var match = FirstOrDefault(key);
      return match.Equals(default) ? default : match.value;
    }
    public virtual void Add(K key, V value) => AddKeyValue(new KeyValue(key, value));

    public virtual bool Remove(K key) => RemoveByKey(key);

    public virtual bool Contains(K key) => !FirstOrDefault(key).Equals(default);

  }
}