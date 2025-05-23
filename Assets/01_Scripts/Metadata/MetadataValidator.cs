using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "MetadataValidator", menuName = "Scriptable Objects/MetadataValidator")]
  public class MetadataValidator : ScriptableObject
  {
    [SerializeField] private List<Metadata.MetadataField> _validationCriteria;
    /// <summary>
    /// Compares all its validation criteria against <paramref name="metadata"/>. 
    /// </summary>
    /// <param name="metadata"></param>
    /// <returns>Returns true if all metadata values are equal to criteria. Else false.</returns>
    public bool Validate(Metadata metadata)
    {
      if (metadata == null) return false;

      var dataToValidate = metadata.Data;
      foreach (var pair in _validationCriteria)
      {
        if (!dataToValidate.ContainsKey(pair.Key)) return false;
        if (dataToValidate[pair.Key] != pair.Value) return false;
      }

      return true;
    }
  }
}
