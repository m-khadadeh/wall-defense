using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  public abstract class WallSegmentYield : TaskYield
  {
    [SerializeField] private List<Entry> _wallSections;
    protected Dictionary<string, WallSegmentName> _dictionary;

    [Serializable]
    public class Entry
    {
      [field: SerializeField] public string ChoiceName { get; private set; }
      [field: SerializeField] public WallSegmentName WallSegment { get; private set; }
    }

    protected void InitializeDictionaryIfNot()
    {
      if (_dictionary == null)
      {
        _dictionary = new Dictionary<string, WallSegmentName>();
        foreach (var section in _wallSections)
        {
          _dictionary.Add(section.ChoiceName, section.WallSegment);
        }
      }
    }
  }
}
