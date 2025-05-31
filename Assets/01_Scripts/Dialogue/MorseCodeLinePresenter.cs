using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace WallDefense
{
  public class MorseCodeLinePresenter : DialoguePresenterBase
  {
    [SerializeField] private TMPro.TextMeshProUGUI _lineTextMorse;
    [SerializeField] private TMPro.TextMeshProUGUI _lineTextNonMorse;
    [SerializeField] private TMPro.TextMeshProUGUI _characterTextMorse;
    [SerializeField] private TMPro.TextMeshProUGUI _characterTextNonMorse;
    [SerializeField] private GameObject _dialogueContainerMorse;
    [SerializeField] private GameObject _dialogueContainerNonMorse;
    [SerializeField] private int _ditMilliseconds;
    [SerializeField] private int _dahDitLength;
    [SerializeField] private int _characterEndDitLength;
    [SerializeField] private int _spaceEndDitLength;
    [SerializeField] private MorseCodeDictionary _dictionary;
    [SerializeField] private string _morseCodeRTFOpenTags;
    [SerializeField] private string _morseCodeRTFClosingTags;
    [SerializeField] private string _morseCodeCharacterPrefix;
    [SerializeField] private int _nonMorsePerCharacterMilliseconds;

    public override YarnTask OnDialogueCompleteAsync()
    {
      return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueStartedAsync()
    {
      return YarnTask.CompletedTask;
    }

    public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
    {
      if (_lineTextMorse == null || _lineTextNonMorse == null)
      {
        Debug.LogError($"Line view doesn't have text view. Skipping line {line.TextID} (\"{line.RawText}\")");
        return;
      }
      if (_characterTextMorse == null || _characterTextNonMorse == null)
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

      bool morse = line.CharacterName.StartsWith(_morseCodeCharacterPrefix);
      if (morse)
      {
        _dialogueContainerMorse.SetActive(true);
        _dialogueContainerNonMorse.SetActive(false);
        _characterTextMorse.text = line.CharacterName.Replace(_morseCodeCharacterPrefix, "");
        string stringToWrite = text.Text.ToUpper();

        ParseData newTextData = new ParseData();
        newTextData.plainText = stringToWrite;
        newTextData.replacedUpperLineText = stringToWrite;
        newTextData.prosignIndicesAndLengths = new Dictionary<int, int>();

        string pattern = @"\((?<prosign>[A-Z]+)\)";
        Regex rg = new Regex(pattern);
        Match match = rg.Match(newTextData.plainText);
        while (match.Success)
        {
          newTextData.plainText = newTextData.plainText.Substring(0, match.Index) + newTextData.plainText.Substring(match.Index + 1, match.Length - 2) +
              ((match.Index + match.Length == newTextData.plainText.Length) ? "" : newTextData.plainText.Substring(match.Index + match.Length));
          Match styled = rg.Match(newTextData.replacedUpperLineText);
          newTextData.replacedUpperLineText = newTextData.replacedUpperLineText.Substring(0, styled.Index) + "<s>" + newTextData.replacedUpperLineText.Substring(styled.Index + 1, styled.Length - 2) + "</s>" +
              ((styled.Index + styled.Length == newTextData.replacedUpperLineText.Length) ? "" : newTextData.replacedUpperLineText.Substring(styled.Index + styled.Length));
          newTextData.prosignIndicesAndLengths.Add(match.Index, match.Length - 2);
          match = rg.Match(newTextData.plainText);
        }

        _lineTextMorse.maxVisibleCharacters = 0;

        if (_ditMilliseconds > 0)
        {
          int currentMorseLength = 1;
          int extraTagPadding = 0;
          for (int i = 0; i < newTextData.plainText.Length; i += currentMorseLength)
          {
            string currentSubstr = newTextData.replacedUpperLineText.Substring(0, i + extraTagPadding);
            if (newTextData.prosignIndicesAndLengths.TryGetValue(i, out currentMorseLength))
            {
              extraTagPadding += 7;
            }
            else
            {
              currentMorseLength = 1;
            }
            _lineTextMorse.text = currentSubstr;
            if (currentMorseLength == 1 ? _dictionary.CharacterCodes.ContainsKey(newTextData.plainText[i]) : _dictionary.ProsignCodes.ContainsKey(newTextData.plainText.Substring(i, currentMorseLength)))
            {
              // Character exists
              string currentCharacterCode = currentMorseLength == 1 ? _dictionary.CharacterCodes[newTextData.plainText[i]] : _dictionary.ProsignCodes[newTextData.plainText.Substring(i, currentMorseLength)];
              _lineTextMorse.text += _morseCodeRTFOpenTags + currentCharacterCode + _morseCodeRTFClosingTags;
              for (int j = 0; j <= currentCharacterCode.Length; j++)
              {
                _lineTextMorse.maxVisibleCharacters = i + j;
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
                  if (!token.IsNextLineRequested)
                    AudioManager.PlayMorseWave(pauseLength);
                }
                await YarnTask.Delay(TimeSpan.FromMilliseconds(pauseLength), token.HurryUpToken).SuppressCancellationThrow();
              }
              if (!token.IsNextLineRequested)
                AudioManager.PlaySound("typewriter");
            }
            else
            {
              // Character doesn't exist, treat as a space.
              await YarnTask.Delay(TimeSpan.FromMilliseconds(_ditMilliseconds * _spaceEndDitLength), token.HurryUpToken).SuppressCancellationThrow();
            }
          }
        }
        _lineTextMorse.text = newTextData.replacedUpperLineText.ToUpper();
        _lineTextMorse.maxVisibleCharacters = newTextData.replacedUpperLineText.Length;
      }
      else
      {
        _dialogueContainerNonMorse.SetActive(true);
        _dialogueContainerMorse.SetActive(false);
        _characterTextNonMorse.text = line.CharacterName;
        _lineTextNonMorse.text = text.Text;
        _lineTextNonMorse.maxVisibleCharacters = 0;
        if (_nonMorsePerCharacterMilliseconds > 0)
        {
          for (int i = 0; i < _lineTextNonMorse.text.Length; i++)
          {
            _lineTextNonMorse.maxVisibleCharacters++;
            if (!token.IsNextLineRequested)
              AudioManager.PlaySound("typewriter");
            await YarnTask.Delay(TimeSpan.FromMilliseconds(_nonMorsePerCharacterMilliseconds), token.HurryUpToken).SuppressCancellationThrow();
          }
        }
        else
        {
          _lineTextNonMorse.maxVisibleCharacters = _lineTextNonMorse.text.Length;
        }
      }
      await YarnTask.WaitUntilCanceled(token.NextLineToken).SuppressCancellationThrow();
      _dialogueContainerNonMorse.SetActive(false);
      _dialogueContainerMorse.SetActive(false);
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

    private class ParseData
    {
      public string plainText;
      public string replacedUpperLineText;
      public Dictionary<int, int> prosignIndicesAndLengths;
    }
  }
}
