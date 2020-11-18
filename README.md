# unityTalk
A talk system that highlights words in your paragraphs very easy.

## How it looks?
Let's choose colors for some important words!

![](ReadmeFiles/TalkCloudDemo2.gif)

## Simple to use
Just a few method calls and you will be ready to show some awesome text!
```c#
using Talk;

namespace MyProject
{
    public class Example : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!TalkSystem.Inst.TalkStarted)
                {
                    TalkSystem.Inst.StartTalk();
                }
                else
                {
                    TalkSystem.Inst.NextPage();
                }
            }
        }
    }
}
```

## But, that's it?
This is just starting, These are some of the features that will be added soon.

- [x] Localization Support
- [ ] A Robust Editor.
   - [x] Write and hightlight the most important words.
   - [ ] Import/Export talk files.
- [x] Animations to the words.
