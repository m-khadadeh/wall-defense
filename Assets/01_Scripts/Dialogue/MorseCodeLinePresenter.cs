using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace WallDefense
{
  public class MorseCodeLinePresenter : DialoguePresenterBase
  {
    [SerializeField] private TMPro.TextMeshProUGUI _lineText;
    [SerializeField] private TMPro.TextMeshProUGUI _characterText;
    [SerializeField] private GameObject _dialogueContainer;
    [SerializeField] private int _ditMilliseconds;
    [SerializeField] private int _dahDitLength;
    [SerializeField] private int _characterEndDitLength;
    [SerializeField] private int _spaceEndDitLength;
    [SerializeField] private MorseCodeDictionary _dictionary;
    [SerializeField] private string _morseCodeRTFOpenTags;
    [SerializeField] private string _morseCodeRTFClosingTags;

    public override YarnTask OnDialogueCompleteAsync()
    {
      _dialogueContainer.SetActive(false);
      return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueStartedAsync()
    {
      _dialogueContainer.SetActive(true);
      return YarnTask.CompletedTask;
    }

    public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
    {
      _dialogueContainer.SetActive(true);
      if (_lineText == null)
      {
        Debug.LogError($"Line view doesn't have text view. Skipping line {line.TextID} (\"{line.RawText}\")");
        return;
      }
      if (_characterText == null)
      {
        Debug.LogError($"Line view doesn't have character view. Skipping line {line.TextID} (\"{line.RawText}\")");
        return;
      }

      var text = line.TextWithoutCharacterName;
      text = line.TextWithoutCharacterName;
      if (line.Text.TryGetAttributeWithName("character", out var characterAttribute))
      {
        text.Attributes.Add(characterAttribute);
      }

      _characterText.text = line.CharacterName;
      string stringToWrite = text.Text.ToUpper();

      _lineText.maxVisibleCharacters = 0;

      if (_ditMilliseconds > 0)
      {
        for (int i = 0; i < stringToWrite.Length; i++)
        {
          _lineText.text = stringToWrite.Substring(0, i);
          if (_dictionary.CharacterCodes.ContainsKey(stringToWrite[i]))
          {
            // Character exists
            string currentCharacterCode = _dictionary.CharacterCodes[stringToWrite[i]];
            _lineText.text += _morseCodeRTFOpenTags + currentCharacterCode + _morseCodeRTFClosingTags;
            for (int j = 0; j <= currentCharacterCode.Length; j++)
            {
              _lineText.maxVisibleCharacters = i + j;
              int pauseLength = _ditMilliseconds;
              if (j == currentCharacterCode.Length)
              {
                pauseLength *= _characterEndDitLength;
              }
              else if (currentCharacterCode[j] == '-')
              {
                pauseLength *= _dahDitLength;
              }
              if (j != currentCharacterCode.Length)
              {
                AudioManager.PlayMorseWave(pauseLength);
              }
              await YarnTask.Delay(TimeSpan.FromMilliseconds(pauseLength), token.HurryUpToken).SuppressCancellationThrow();
            }
            AudioManager.PlaySound("typewriter");
          }
          else
          {
            // Character doesn't exist, treat as a space.
            await YarnTask.Delay(TimeSpan.FromMilliseconds(_ditMilliseconds * _spaceEndDitLength), token.HurryUpToken).SuppressCancellationThrow();
          }
        }
      }
      _lineText.text = stringToWrite.ToUpper();
      _lineText.maxVisibleCharacters = stringToWrite.Length;
      

      await YarnTask.WaitUntilCanceled(token.NextLineToken).SuppressCancellationThrow();
      _dialogueContainer.SetActive(false);
    }

    public override YarnTask<DialogueOption> RunOptionsAsync(DialogueOption[] dialogueOptions, CancellationToken cancellationToken)
    {
      return YarnTask<DialogueOption>.FromResult(null);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      _dictionary.Initialize();
      Debug.Log("Initialized");
    }

    // Update is called once per frame
    void Update()
    {
    
    }
  }
}
