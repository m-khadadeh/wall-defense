using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "Metadata", menuName = "Scriptable Objects/Metadata")]
  public class Metadata : ScriptableObject
  {
    [Serializable]
    public class MetadataField
    {
      [field: SerializeField] public string Key { get; set; }
      [field: SerializeField] public string Value { get; set; }
    }
    [SerializeField] private List<MetadataField> _metadataKeyValuePairs;
    private Dictionary<string, string> _data;
    public Dictionary<string, string> Data
    {
      get
      {
        if (_data == null)
        {
          Initialize();
        }
        return _data;
      }
    }

    public void Initialize()
    {
      _data = new Dictionary<string, string>();
      foreach (var pair in _metadataKeyValuePairs)
      {
        _data.Add(pair.Key, pair.Value);
      }
    }
  }
}
