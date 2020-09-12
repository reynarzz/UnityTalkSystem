﻿//MIT License

//Copyright (c) 2020 Reynardo Perez (Reynarz)

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace TalkSystem
{
    public class RuntimeTextExample : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _spaceText;

        private TalkData _data;

        private void Start()
        {
            var page = new TextPage("Esta es la primera linea", default, new Highlight(3, "", Color.blue, HighlightAnimation.None));

            _data = new TalkData(new List<TextPage>() { page }) { Language = Language.Spanish, TalkName = "SpanishTalk" };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!Talker.Inst.IsTalking)
                {
                    Talker.Inst.StartTalk(_data, Events);
                }
                else
                {
                    Talker.Inst.NextPage();
                }
            }
        }

        private void Events(TalkEvent talkEvent)
        {
            switch (talkEvent)
            {
                case TalkEvent.Started:
                    _spaceText.enabled = false;
                    break;
                case TalkEvent.Finished:
                    _spaceText.enabled = true;
                    break;
                case TalkEvent.PageChanged:
                    break;
            }
        }
    }
}
