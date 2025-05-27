using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "MorseCodeDictionary", menuName = "Scriptable Objects/MorseCodeDictionary")]
  public class MorseCodeDictionary : ScriptableObject
  {
    [SerializeField] private List<Entry> _entries;
    private Dictionary<char, string> _characterCodes;
    private Dictionary<string, string> _prosignCodes;

    public Dictionary<char, string> CharacterCodes
    {
      get
      {
        if (_characterCodes == null)
        {
          Initialize();
        }
        return _characterCodes;
      }
    }
    public Dictionary<string, string> ProsignCodes
    {
      get
      {
        if (_prosignCodes == null)
        {
          Initialize();
        }
        return _prosignCodes;
      }
    }

    public void Initialize()
    {
      _characterCodes = new Dictionary<char, string>();
      _prosignCodes = new Dictionary<string, string>();
      foreach (var entry in _entries)
      {
        if (entry.KeyCode.Length == 1)
        {
          _characterCodes.Add(entry.KeyCode[0], entry.MorseCode);
        }
        else
        {
          _prosignCodes.Add(entry.KeyCode, entry.MorseCode);
        }
      }
    }

    [Serializable]
    public class Entry
    {
      [field: SerializeField] public string KeyCode { get; private set; }
      [field: SerializeField] public string MorseCode { get; private set; }
    }  
  }
}
