using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  public class AudioManager : MonoBehaviour
  {
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioClipLibrary _musicClips;
    [SerializeField] private AudioClipLibrary _soundClips;
    [SerializeField] private float _sfxVolume;
    [SerializeField] private float _morseCodeVolume;
    [SerializeField] private string _morseCodeSFXKey;
    private static AudioManager _instance;
    void Awake()
    {
      // Ensure one instance that doesn't disappear between scenes.
      if (_instance != null)
      {
        Destroy(gameObject);
      }
      else
      {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        _musicClips.Initialize();
        _soundClips.Initialize();
      }
    }

    /// <summary>
    /// Play Audioclip from music audio library with the key "songKey"
    /// </summary>
    /// <param name="songKey">The key for the clip to play from the music library</param>
    public static void PlayMusic(string songKey)
    {
      if (_instance._musicClips.Clips.TryGetValue(songKey, out AudioClip clip))
      {
        _instance._musicSource.clip = clip;
        _instance._musicSource.Play();
      }
      else
      {
        Debug.LogError($"Couldn't find music clip with key \"{songKey}\"");
      }
    }

    /// <summary>
    /// Play Audioclip from sound effects audio library with the key "soundKey"
    /// </summary>
    /// <param name="soundKey">The key for the clip to play from the sound library</param>
    public static void PlaySound(string soundKey)
    {
      if (_instance._soundClips.Clips.TryGetValue(soundKey, out AudioClip clip))
      {
        PlaySFX(clip, _instance._sfxVolume);
      }
      else
      {
        Debug.LogError($"Couldn't find sound clip with key \"{soundKey}\"");
      }
    }

    /// <summary>
    /// Plays a sine wave for a specified length in milliseconds.
    /// </summary>
    /// <param name="milliseconds">The amount of milliseconds to play the sine wave for</param>
    public static void PlayMorseWave(int milliseconds)
    {
      if (_instance._soundClips.Clips.TryGetValue(_instance._morseCodeSFXKey, out AudioClip clip))
      {
        PlaySFX(clip, _instance._morseCodeVolume, milliseconds / 1000.0f);
      }
      else
      {
        Debug.LogError($"Couldn't find morse code sound clip with key \"{_instance._morseCodeSFXKey}\"");
      }
    }

    private static void PlaySFX(AudioClip clip, float volume) => PlaySFX(clip, volume, clip.length);

    private static void PlaySFX(AudioClip clip, float volume, float clipLength)
    {
      GameObject newSFXObject = new GameObject();
      newSFXObject.transform.SetParent(_instance.transform);
      AudioSource newAudioSource = newSFXObject.AddComponent<AudioSource>();
      newAudioSource.volume = volume;
      newAudioSource.PlayOneShot(clip);

      Destroy(newSFXObject, clipLength);
    }
  }
}
