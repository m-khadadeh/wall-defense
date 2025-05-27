using System;
using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "AudioClipLibrary", menuName = "Scriptable Objects/AudioClipLibrary")]
  public class AudioClipLibrary : ScriptableObject
  {
    [SerializeField] private List<Entry> _clips;
    public Dictionary<string, AudioClip> Clips { get; private set; }

    public void Initialize()
    {
      Clips = new Dictionary<string, AudioClip>();
      foreach (var clip in _clips)
      {
        Clips.Add(clip.ClipName, clip.Clip);
      }
    }

    [Serializable]
    public class Entry
    {
      [field: SerializeField] public string ClipName { get; private set; }
      [field: SerializeField] public AudioClip Clip { get; private set; }
    }
  }
}
