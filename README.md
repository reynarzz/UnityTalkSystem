# UTalk  ![](https://img.shields.io/badge/Release-Beta-green) ![](https://img.shields.io/badge/Licence-MIT-lightgrey)
An Unity3D talk system that highlights words in your text very easy.

## How it looks?
Let's choose some colors for all the important words!

![](ReadmeFiles/preview.gif)

## Open the editor: Window > TalkEditor
![](ReadmeFiles/editorDemo.gif)

## Easy to implement!
Just a few method calls and you will be ready to show some awesome text!
```c#
using uTalk;

namespace MyProject
{
    public class Example : MonoBehaviour
    {
    	[SerializeField] private UTalk _utalk;
	[SerializeField] private TalkCloudBase _talkCloud;
        
	private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!_utalk.IsTalking)
                {
		    var talkInfo = new TalkInfo("Group", "SubGroup", "TalkName", "Language");

                    _utalk.StartTalk(_talkCloud, talkInfo);
                }
                else
                {
                    _utalk.NextPage();
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
   - [x] Write and hightlight the words.
   - [ ] Import/Export talk files.
- [x] Animations to the words.

## Running ver?
2020.1.9f1, but newer versions should work as well.
