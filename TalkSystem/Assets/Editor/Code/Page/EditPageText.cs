﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace TalkSystem.Editor
{
    public class EditPageText : IPage
    {
        public delegate void TextChanged(string text, int charsAdded, int cursor);

        private string _text;
        private int _oldCursorIndex;
        private (int, string) _selectedWord;

        private GUIStyle _labelStyle;

        private TalkData _talkData;

        private int _textPageIndex = -1;

        public event TextChanged OnTextChanged;

        private TextPage _currentTextPage;
        private StringBuilder _highlightedText;

        private class Clipboard
        {
            private List<Highlight> _highlightClipboard;

            public Clipboard()
            {
                _highlightClipboard = new List<Highlight>();
            }

            public void SetToClipBoard(TextPage page, string fullText, string copiedText, int selectIndex, int cursor)
            {
                int startCharIndex;
                int endCharIndex;

                _highlightClipboard.Clear();

                if (cursor > selectIndex)
                {
                    startCharIndex = selectIndex;
                    endCharIndex = cursor;
                }
                else
                {
                    startCharIndex = cursor;
                    endCharIndex = selectIndex;
                }

                for (int i = startCharIndex; i < endCharIndex; i++)
                {
                    var prevChar = fullText.ElementAtOrDefault(i - 1);

                    var prevCharIsValid = prevChar == default || prevChar == ' ' || prevChar == '\n';
                    var currentCharIsValid = fullText[i] != ' ' && fullText[i] != '\n' && i < endCharIndex;

                    if (prevCharIsValid && currentCharIsValid)
                    {
                        //Does this word has a highlight property?
                        if (page.Highlight.ContainsKey(i))
                        {
                            _highlightClipboard.Add(page.Highlight[i]);
                        }

                        Debug.Log("Starting char " + i);
                    }
                }
            }

            public void PasteHighlightOfClipboard()
            {
                //TODO
            }

            public List<Highlight> GetHighlightClipboard()
            {
                return default;
            }
        }

        private Clipboard _clipboard;

        public EditPageText()
        {
            _highlightedText = new StringBuilder();
            _clipboard = new Clipboard();

            OnTextChanged += OnTextChangedHandler;
        }


        public void OnGUI(TalkData talkData)
        {
            _talkData = talkData;

            if (_textPageIndex >= 0)
            {
                Init();

                GUILayout.Space(10);

                var textInfo = GUIUtils.TextArea(ref _text, SetToClipboard);

                if (textInfo.TextLengthChanged)
                {
                    OnTextChanged(textInfo.Text, textInfo.AddedChars, textInfo.CursorIndex);
                }

                GUILayout.Space(5);

                UpdateColor(textInfo);

                var page = new TextPage(_text, _currentTextPage.Sprite, _currentTextPage.Event, _currentTextPage.Highlight);

                var hightligted = HighlightText(page);

                GUILayout.Space(5);
                TextPreview(hightligted);

                if (GUILayout.Button("Show highlight info"))
                {
                    var orderedKeys = _currentTextPage.Highlight.Keys.OrderBy(x => x);

                    for (int i = 0; i < _currentTextPage.Highlight.Count; i++)
                    {
                        var key = orderedKeys.ElementAt(i);
                        var highlight = _currentTextPage.Highlight[key];

                        Debug.Log("Key: " + key + ", charIndex: " + highlight.WordCharIndex + ", wordIndex: " + Highlight.GetWordIndex(_text, highlight.WordCharIndex));
                    }
                }
            }
        }

        private void SetToClipboard(GUIUtils.TextOperation operation, string clipboardText, int selectIndex, int cursor)
        {
            switch (operation)
            {
                case GUIUtils.TextOperation.Copy:
                case GUIUtils.TextOperation.Cut:
                    _clipboard.SetToClipBoard(_currentTextPage, _text, clipboardText, selectIndex, cursor);
                    break;
                case GUIUtils.TextOperation.Paste:
                    break;
            }
        }

        private void UpdateColor(GUIUtils.TextEditorInfo textInfo)
        {
            if (_oldCursorIndex != textInfo.CursorIndex)
            {
                _oldCursorIndex = textInfo.CursorIndex;

                var word = Highlight.GetWordIndex(_text, textInfo.CursorIndex);

                if (!string.IsNullOrEmpty(word.Item2))
                {
                    _selectedWord = word;

                    //Debug.Log(word);
                }
            }

            //does the TextArea have text?
            if (!string.IsNullOrEmpty(_selectedWord.Item2))
            {
                var startingCharIndex = Highlight.GetStartingCharIndex(_text, _selectedWord.Item1);
                var containsKey = _currentTextPage.Highlight.ContainsKey(startingCharIndex);

                GUILayout.BeginVertical(EditorStyles.helpBox);

                var highlight = default(Highlight);

                if (containsKey)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                    {
                        _currentTextPage.Highlight.Remove(startingCharIndex);

                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        return;
                    }

                    var selected = !string.IsNullOrEmpty(textInfo.SelectedText) ? textInfo.SelectedText : _selectedWord.Item2;

                    GUILayout.Label($"Highlight ({ selected + " : " + _selectedWord.Item1})");

                    GUILayout.EndHorizontal();

                    highlight = _currentTextPage.Highlight[startingCharIndex];
                }
                else if (GUILayout.Button($"Add Hightlight to: {_selectedWord.Item2}"))
                {
                    highlight = new Highlight(startingCharIndex, _selectedWord.Item2.Length, Color.white, HighlightAnimation.None);

                    _currentTextPage.Highlight.Add(startingCharIndex, highlight);
                }

                if (highlight != default)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Color", GUILayout.MaxWidth(70));
                    var color = EditorGUILayout.ColorField(highlight.Color);
                    color.a = 1;
                    GUILayout.EndHorizontal();

                    GUILayout.Space(3);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Animation", GUILayout.MaxWidth(70));
                    var type = (HighlightAnimation)EditorGUILayout.EnumPopup(highlight.Type);
                    GUILayout.EndHorizontal();


                    highlight = new Highlight(startingCharIndex, _selectedWord.Item2.Length, color, type);

                    _currentTextPage.Highlight[startingCharIndex] = highlight;
                }

                GUILayout.EndVertical();
            }
        }

        private void Init()
        {
            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.label);
                _labelStyle.active.textColor = Color.white;
                _labelStyle.normal.textColor = Color.white;
                _labelStyle.richText = true;
                _labelStyle.wordWrap = true;
            }
        }

        private void TextPreview(string text)
        {
            GUILayout.Label("Preview");

            GUILayout.BeginVertical(EditorStyles.helpBox);


            GUILayout.Label(text, _labelStyle);
            GUILayout.EndVertical();
        }



        public void SetTextPageIndex(int textPageIndex)
        {
            _textPageIndex = textPageIndex;

            _currentTextPage = _talkData.GetPage(_textPageIndex);

            _text = _currentTextPage.Text;
        }

        //TODO: detect if a modified text index was changed.
        private void OnTextChangedHandler(string newText, int addedChars, int cursor)
        {
            //basically a word have to be like a pointer, all the properties you put to a word can't be losed if a word chage it's index.
            //var oldText = _currentTextPage.Text;

            //var wasAdded = addedChars > 0;

            var insertedIndex = cursor - addedChars;

            //if (wasAdded)
            //{
            //    //Added to end of text.
            //    if (cursor == newText.Length)
            //    {
            //        Debug.Log("Added to end");
            //    }
            //    else //Inserted in a part of the text.
            //    {
            //        var textInserted = newText.Substring(cursor - addedChars, addedChars);

            //        //var notModifableText = oldText.Substring(0, insertedIndex);
            //        //var textToModify = oldText.Substring(insertedIndex, oldText.Length - insertedIndex);

            //        RearrangeHighlightedWords(addedChars, insertedIndex);


            //        Debug.Log($"Inserted \"{textInserted}\" in: " + insertedIndex + ", Recreated");
            //    }
            //}
            //else
            //{
            //    Debug.Log("Was removed: " + addedChars);

            //    RearrangeHighlightedWords(addedChars, insertedIndex);
            //}

            RearrangeHighlightedWords(addedChars, insertedIndex);

            _currentTextPage = new TextPage(newText, _currentTextPage.Highlight);
        }

        //TODO: if you mix two words, there should be a way to clean up.
        //Always clean: repeated charIndex of highlights, and charIndexes that points to empty chars.
        //Two problems: Clean up of unnuced highlights and word duplicates with different charIndex in the highlight object.
        private void RearrangeHighlightedWords(int addedChars, int insertedIndex)
        {
            if (_currentTextPage.Highlight.Count > 0)
            {
                var highlightKeysToModify = _currentTextPage.Highlight.Keys.Where(x => x >= insertedIndex);

                for (int i = 0; i < highlightKeysToModify.Count(); i++)
                {
                    var key = highlightKeysToModify.ElementAt(i);

                    var newKey = key + addedChars;

                    var highlight = _currentTextPage.Highlight[key];

                    highlight = new Highlight(newKey, highlight.WordLength, highlight.Color, highlight.Type);

                    _currentTextPage.Highlight.Remove(key);

                    if (_currentTextPage.Highlight.ContainsKey(newKey))
                    {
                        Debug.Log("replace key");
                        _currentTextPage.Highlight[newKey] = highlight;
                    }
                    else
                    {
                        _currentTextPage.Highlight.Add(newKey, highlight);
                    }
                }

                //Debug.Log("Count: " + highlightKeysToModify.Count());
            }
        }

        private string HighlightText(TextPage page)
        {
            _highlightedText.Clear();
            _highlightedText.Append(page.Text);

            var splited = Regex.Split(_highlightedText.ToString(), " |\n");

            for (int i = 0; i < page.Highlight.Count; i++)
            {
                var key = page.Highlight.Keys.ElementAt(i);

                //Get a highlight
                var highlight = page.Highlight[key];

                //Get the wordIndex where the highligh is
                var wordIndex = Highlight.GetWordIndex(_highlightedText.ToString(), highlight.WordCharIndex).Item1;

                var hex = ColorUtility.ToHtmlStringRGBA(highlight.Color);

                var colorOpen = $"<color=#{hex}>";
                var colorClose = "</color>";

                var unmmodified = splited[wordIndex];

                unmmodified = unmmodified.Insert(0, colorOpen);
                unmmodified = unmmodified.Insert(unmmodified.Length, colorClose);

                splited[wordIndex] = unmmodified;
            }

            _highlightedText.Clear();

            for (int i = 0; i < splited.Length; i++)
            {
                _highlightedText.Append(splited[i]);
                _highlightedText.Append(" ");

            }

            return _highlightedText.ToString();
        }
    }
}
