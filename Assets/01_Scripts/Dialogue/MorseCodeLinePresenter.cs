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
    [SerializeField] private GameObject _morseKeyOn;
    [SerializeField] private GameObject _morseKeyOff;
    [SerializeField] private string _playerMorseName;
    [SerializeField] private Button _morseNextButton;
    [SerializeField] private TMPro.TextMeshProUGUI[] _optionText;
    [SerializeField] private Button[] _optionButtons;
    private (string, string) _lastLine;
    private bool _lineRunning;

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
      foreach (var button in _optionButtons)
      {
        button.gameObject.SetActive(false);
      }

      var text = line.TextWithoutCharacterName;
      text = line.TextWithoutCharacterName;
      if (line.Text.TryGetAttributeWithName("character", out var characterAttribute))
      {
        text.Attributes.Add(characterAttribute);
      }

      bool morse = line.CharacterName.StartsWith(_morseCodeCharacterPrefix);
      _lineRunning = true;
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
        _lastLine = (_characterTextMorse.text, newTextData.replacedUpperLineText);

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

                int waveLength = _ditMilliseconds;
                if (j != currentCharacterCode.Length && currentCharacterCode[j] == '-')
                {
                  waveLength *= _dahDitLength;
                }
                if (j != currentCharacterCode.Length)
                {
                  if (!token.IsNextLineRequested && !token.IsHurryUpRequested)
                  {
                    AudioManager.PlayMorseWave(waveLength);
                    if (line.CharacterName == $"{_morseCodeCharacterPrefix}{_playerMorseName}")
                    {
                      _morseKeyOn.SetActive(true);
                      _morseKeyOff.SetActive(false);
                    }
                  }
                  await YarnTask.Delay(TimeSpan.FromMilliseconds(waveLength), token.HurryUpToken).SuppressCancellationThrow();
                  _morseKeyOn.SetActive(false);
                  _morseKeyOff.SetActive(true);
                  await YarnTask.Delay(TimeSpan.FromMilliseconds(_ditMilliseconds), token.HurryUpToken).SuppressCancellationThrow();
                }
                else
                {
                  await YarnTask.Delay(TimeSpan.FromMilliseconds(_ditMilliseconds * _characterEndDitLength), token.HurryUpToken).SuppressCancellationThrow();
                }
              }
              if (!token.IsNextLineRequested && !token.IsHurryUpRequested)
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
        _lastLine = (line.CharacterName, text.Text);
        if (_nonMorsePerCharacterMilliseconds > 0)
        {
          for (int i = 0; i < _lineTextNonMorse.text.Length; i++)
          {
            _lineTextNonMorse.maxVisibleCharacters++;
            if (!token.IsNextLineRequested && !token.IsHurryUpRequested)
              AudioManager.PlaySound("typewriter");
            await YarnTask.Delay(TimeSpan.FromMilliseconds(_nonMorsePerCharacterMilliseconds), token.HurryUpToken).SuppressCancellationThrow();
          }
        }
        else
        {
          _lineTextNonMorse.maxVisibleCharacters = _lineTextNonMorse.text.Length;
        }
      }
      _lineRunning = false;
      _morseKeyOn.SetActive(false);
      _morseKeyOff.SetActive(true);
      await YarnTask.WaitUntilCanceled(token.NextLineToken).SuppressCancellationThrow();
      _dialogueContainerNonMorse.SetActive(false);
      _dialogueContainerMorse.SetActive(false);
    }

    public void OnNextButtonClicked(DialogueRunner runner)
    {
      if (_lineRunning)
      {
        runner.RequestHurryUpLine();
      }
      else
      {
        runner.RequestNextLine();
      }
    }

    public override async YarnTask<DialogueOption> RunOptionsAsync(DialogueOption[] dialogueOptions, CancellationToken cancellationToken)
    {
      _dialogueContainerMorse.SetActive(true);
      _dialogueContainerNonMorse.SetActive(false);
      _characterTextMorse.text = _lastLine.Item1;
      _lineTextMorse.text = _lastLine.Item2;
      _lineTextMorse.maxVisibleCharacters = _lastLine.Item2.Length;
      _morseNextButton.gameObject.SetActive(false);

      YarnTaskCompletionSource<DialogueOption> selectedOptionCompletionSource = new YarnTaskCompletionSource<DialogueOption>();
      var completionCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

      async YarnTask CancelSourceWhenDialogueCancelled()
      {
        await YarnTask.WaitUntilCanceled(completionCancellationSource.Token);
        if (cancellationToken.IsCancellationRequested == true)
        {
          selectedOptionCompletionSource.TrySetResult(null);
        }
      }

      CancelSourceWhenDialogueCancelled().Forget();
      
      for (int i = 0, j = 0; i < dialogueOptions.Length; i++)
      {
        DialogueOption thisOption = dialogueOptions[i];
        if (thisOption.IsAvailable)
        {
          _optionButtons[j].gameObject.SetActive(true);
          _optionText[j].text = thisOption.Line.Text.Text;
          _optionButtons[j].onClick.RemoveAllListeners();
          _optionButtons[j].onClick.AddListener(() =>
          {
            if (!completionCancellationSource.IsCancellationRequested)
            {
              selectedOptionCompletionSource.TrySetResult(thisOption);
            }
          });
          j++;
        }
      }

      var completedTask = await selectedOptionCompletionSource.Task;
      completionCancellationSource.Cancel();

      if (cancellationToken.IsCancellationRequested)
      {
        return await DialogueRunner.NoOptionSelected;
      }

      foreach (var button in _optionButtons)
      {
        button.onClick.RemoveAllListeners();
        button.gameObject.SetActive(false);
      }
      
      _morseNextButton.gameObject.SetActive(true);
      _dialogueContainerMorse.SetActive(false);

      return completedTask;
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
